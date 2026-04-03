using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using CommEx.Infrastructure.Logging;
using CommEx.Models;

namespace CommEx.Services
{
    /// <summary>
    /// ポート単位の独立ワーカーを管理する COM 通信サービスです。
    /// </summary>
    internal sealed class ComCommunicationService : IComCommunicationService
    {
        private readonly IPluginLogger logger;
        private readonly object syncRoot = new object();
        private readonly List<PortWorker> workers = new List<PortWorker>();

        private bool started;

        public ComCommunicationService(IPluginLogger logger)
        {
            this.logger = logger;
        }

        public void Start(ComTransportModel model)
        {
            if (model == null || !model.Enabled)
            {
                return;
            }

            lock (syncRoot)
            {
                if (started)
                {
                    return;
                }

                foreach (ComPortConfiguration port in model.Ports)
                {
                    PortWorker worker = new PortWorker(port, logger);
                    workers.Add(worker);
                    worker.Start();
                }

                started = true;
            }
        }

        public void PublishSnapshot(TelemetrySnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            lock (syncRoot)
            {
                if (!started)
                {
                    return;
                }

                foreach (PortWorker worker in workers)
                {
                    worker.Enqueue(snapshot);
                }
            }
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                foreach (PortWorker worker in workers)
                {
                    worker.Dispose();
                }

                workers.Clear();
                started = false;
            }
        }

        private sealed class PortWorker : IDisposable
        {
            private readonly ComPortConfiguration configuration;
            private readonly IPluginLogger logger;
            private readonly BlockingCollection<TelemetrySnapshot> queue = new BlockingCollection<TelemetrySnapshot>(64);
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            private Task workerTask;

            public PortWorker(ComPortConfiguration configuration, IPluginLogger logger)
            {
                this.configuration = configuration;
                this.logger = logger;
            }

            public void Start()
            {
                workerTask = Task.Factory.StartNew(
                    WorkerLoop,
                    cancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }

            public void Enqueue(TelemetrySnapshot snapshot)
            {
                if (!queue.IsAddingCompleted)
                {
                    queue.TryAdd(snapshot);
                }
            }

            public void Dispose()
            {
                queue.CompleteAdding();
                cancellationTokenSource.Cancel();

                if (workerTask != null)
                {
                    try
                    {
                        workerTask.Wait(TimeSpan.FromSeconds(1));
                    }
                    catch (AggregateException)
                    {
                    }
                }

                queue.Dispose();
                cancellationTokenSource.Dispose();
            }

            private void WorkerLoop()
            {
                try
                {
                    if (!configuration.AutoStart)
                    {
                        return;
                    }

                    using (SerialPort serialPort = BuildPort(configuration))
                    {
                        TryOpen(serialPort);

                        foreach (TelemetrySnapshot snapshot in queue.GetConsumingEnumerable(cancellationTokenSource.Token))
                        {
                            if (!serialPort.IsOpen)
                            {
                                continue;
                            }

                            string frame = SerializeFrame(configuration.ProtocolType, snapshot);
                            serialPort.Write(frame);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    logger.Info($"COM worker stopped: {configuration.PortName} / {ex.Message}");
                }
            }

            private static SerialPort BuildPort(ComPortConfiguration configuration)
            {
                return new SerialPort(configuration.PortName)
                {
                    BaudRate = configuration.BaudRate,
                    DataBits = configuration.DataBits,
                    StopBits = configuration.StopBits,
                    Parity = configuration.Parity,
                    DtrEnable = configuration.DtrEnable,
                    RtsEnable = configuration.RtsEnable,
                    NewLine = "\r\n",
                };
            }

            private void TryOpen(SerialPort serialPort)
            {
                try
                {
                    serialPort.Open();
                    logger.Info($"COM port opened: {configuration.PortName} ({configuration.ProtocolType})");
                }
                catch (Exception ex)
                {
                    logger.Info($"COM port open failed: {configuration.PortName} / {ex.Message}");
                }
            }

            private static string SerializeFrame(ComProtocolType protocolType, TelemetrySnapshot snapshot)
            {
                switch (protocolType)
                {
                    case ComProtocolType.Bids:
                        return $"TRI{snapshot.TickCount}\r\n";
                    case ComProtocolType.CommunicationDll:
                        return $"CDLL,{snapshot.TickCount},{snapshot.TickUtc:O}\r\n";
                    case ComProtocolType.BveSerialOutput:
                        return $"{snapshot.TickCount:D6}{snapshot.TickUtc:HHmmss}\\r";
                    case ComProtocolType.CustomBinary:
                        return string.Concat(snapshot.TickCount.ToString("X8"), "\r\n");
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
