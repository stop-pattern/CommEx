using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using BveExCsTemplate.Extension.Model;

namespace BveExCsTemplate.Extension.Infrastructure
{
    /// <summary>
    /// 通信処理を BveEx の Tick スレッドから分離するワーカー。
    /// </summary>
    internal sealed class CommunicationWorker : IDisposable
    {
        private readonly IReadOnlyList<IFramePublisher> publishers;
        private readonly BlockingCollection<SimulationFrame> queue;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Thread workerThread;

        public CommunicationWorker(IReadOnlyList<IFramePublisher> publishers, int capacity = 64)
        {
            this.publishers = publishers ?? throw new ArgumentNullException(nameof(publishers));
            queue = new BlockingCollection<SimulationFrame>(capacity);
            workerThread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "CommEx.CommunicationWorker",
            };
        }

        public void Start()
        {
            workerThread.Start();
        }

        /// <summary>
        /// キュー満杯時は最新フレーム優先で古いデータを捨てる（BveEx 側をブロックしない）。
        /// </summary>
        public void EnqueueLatest(SimulationFrame frame)
        {
            if (frame == null) return;

            while (!queue.TryAdd(frame))
            {
                SimulationFrame dropped;
                queue.TryTake(out dropped);
            }
        }

        private void WorkerLoop()
        {
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    SimulationFrame frame;
                    try
                    {
                        frame = queue.Take(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    foreach (var publisher in publishers)
                    {
                        try
                        {
                            publisher.Publish(frame);
                        }
                        catch
                        {
                            // 通信失敗は握りつぶして次の宛先へ進む
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            queue.CompleteAdding();
            if (workerThread.IsAlive)
            {
                workerThread.Join(TimeSpan.FromSeconds(1));
            }

            foreach (var publisher in publishers)
            {
                var disposable = publisher as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            queue.Dispose();
            cts.Dispose();
        }
    }
}
