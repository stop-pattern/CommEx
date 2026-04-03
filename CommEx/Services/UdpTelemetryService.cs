using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Models;

namespace CommEx.Services
{
    /// <summary>
    /// UDP で外部へテレメトリを送信するサービスです。
    /// </summary>
    internal class UdpTelemetryService : ITelemetryService
    {
        private readonly IPluginLogger logger;
        private readonly IClock clock;
        private readonly UdpTransportSettings settings;
        private readonly BlockingCollection<UdpTelemetryPacket> sendQueue;
        private readonly Thread workerThread;
        private readonly IPEndPoint destination;

        private volatile bool isDisposed;

        public UdpTelemetryService(IPluginLogger logger, IClock clock, UdpTransportSettings settings)
        {
            this.logger = logger;
            this.clock = clock;
            this.settings = settings;

            sendQueue = new BlockingCollection<UdpTelemetryPacket>(new ConcurrentQueue<UdpTelemetryPacket>(), 256);
            destination = new IPEndPoint(IPAddress.Parse(settings.DestinationIp), settings.DestinationPort);

            workerThread = new Thread(WorkLoop)
            {
                IsBackground = true,
                Name = "CommEx-UdpWorker"
            };
            workerThread.Start();
        }

        public void PublishHeartbeat(int tickCount, TimeSpan elapsed)
        {
            if (tickCount == 1)
            {
                logger.Info("UDP telemetry service initialized at " + clock.UtcNow.ToString("O"));
            }

            _ = elapsed;
        }

        public void PublishUdpTelemetry(UdpTelemetryPacket packet)
        {
            if (!settings.IsEnabled || isDisposed)
            {
                return;
            }

            if (!sendQueue.TryAdd(packet))
            {
                logger.Info("UDP queue is full. Dropped 1 telemetry packet.");
            }
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            sendQueue.CompleteAdding();
            if (!workerThread.Join(TimeSpan.FromSeconds(1)))
            {
                logger.Info("UDP worker did not stop within timeout.");
            }

            sendQueue.Dispose();
        }

        private void WorkLoop()
        {
            try
            {
                using (UdpClient udpClient = settings.LocalPort > 0 ? new UdpClient(settings.LocalPort) : new UdpClient())
                {
                    foreach (UdpTelemetryPacket packet in sendQueue.GetConsumingEnumerable())
                    {
                        byte[] payload = Encoding.UTF8.GetBytes(packet.ToCsvPayload());
                        udpClient.Send(payload, payload.Length, destination);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Info("UDP worker stopped by exception: " + ex.Message);
            }
        }
    }
}
