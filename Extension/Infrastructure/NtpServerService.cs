using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using BveExCsTemplate.Extension.Model;

namespace BveExCsTemplate.Extension.Infrastructure
{
    /// <summary>
    /// BveEx 時刻を返す簡易 NTP サーバー。
    /// </summary>
    internal sealed class NtpServerService : IDisposable
    {
        private readonly UdpClient udp;
        private readonly BveExModelStore model;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private Task loopTask;

        public NtpServerService(BveExModelStore model, int port)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            udp = new UdpClient(port);
        }

        public void Start()
        {
            loopTask = Task.Run((Func<Task>)LoopAsync);
        }

        private async Task LoopAsync()
        {
            while (!cts.IsCancellationRequested)
            {
                UdpReceiveResult recv;
                try
                {
                    recv = await udp.ReceiveAsync().ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException)
                {
                    break;
                }

                var response = NtpPacketCodec.CreateServerResponse(
                    recv.Buffer,
                    model.GetVirtualNowUtc(DateTimeOffset.UtcNow));

                await udp.SendAsync(response, response.Length, recv.RemoteEndPoint).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            udp.Dispose();
            if (loopTask != null)
            {
                try
                {
                    loopTask.Wait(TimeSpan.FromSeconds(1));
                }
                catch (AggregateException)
                {
                }
            }
            cts.Dispose();
        }
    }

    internal static class NtpPacketCodec
    {
        private static readonly DateTimeOffset NtpEpoch = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static byte[] CreateServerResponse(byte[] request, DateTimeOffset nowUtc)
        {
            var response = new byte[48];
            response[0] = 0x24; // LI=0, VN=4, Mode=4(server)
            response[1] = 1; // stratum
            response[2] = 0; // poll
            response[3] = unchecked((byte)-20); // precision

            // originate timestamp = request transmit timestamp
            if (request != null && request.Length >= 48)
            {
                Buffer.BlockCopy(request, 40, response, 24, 8);
            }

            WriteTimestamp(response, 32, nowUtc); // receive timestamp
            WriteTimestamp(response, 40, nowUtc); // transmit timestamp
            return response;
        }

        public static void WriteTimestamp(byte[] packet, int offset, DateTimeOffset value)
        {
            var delta = value - NtpEpoch;
            var seconds = (uint)delta.TotalSeconds;
            var fraction = (uint)((delta.TotalSeconds - Math.Truncate(delta.TotalSeconds)) * uint.MaxValue);

            packet[offset] = (byte)(seconds >> 24);
            packet[offset + 1] = (byte)(seconds >> 16);
            packet[offset + 2] = (byte)(seconds >> 8);
            packet[offset + 3] = (byte)seconds;

            packet[offset + 4] = (byte)(fraction >> 24);
            packet[offset + 5] = (byte)(fraction >> 16);
            packet[offset + 6] = (byte)(fraction >> 8);
            packet[offset + 7] = (byte)fraction;
        }

        public static DateTimeOffset ReadTimestamp(byte[] packet, int offset)
        {
            var seconds =
                ((uint)packet[offset] << 24) |
                ((uint)packet[offset + 1] << 16) |
                ((uint)packet[offset + 2] << 8) |
                packet[offset + 3];

            var fraction =
                ((uint)packet[offset + 4] << 24) |
                ((uint)packet[offset + 5] << 16) |
                ((uint)packet[offset + 6] << 8) |
                packet[offset + 7];

            var fractionSeconds = fraction / (double)uint.MaxValue;
            return NtpEpoch.AddSeconds(seconds + fractionSeconds);
        }
    }
}
