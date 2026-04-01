using System;
using System.Text;
using Newtonsoft.Json;

using BveExCsTemplate.Extension.Model;

namespace BveExCsTemplate.Extension.Infrastructure
{
    internal interface IUdpSender : IDisposable
    {
        void Send(byte[] packet);
    }

    /// <summary>
    /// BveEx 情報をそのまま UDP で送信する。
    /// </summary>
    internal sealed class UdpRelayService : IDisposable
    {
        private readonly IUdpSender sender;

        public UdpRelayService(IUdpSender sender)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public void Publish(SimulationFrame frame)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            var packet = new
            {
                type = "bveex.raw",
                bveTimeUtc = frame.BveTimeUtc.UtcDateTime.ToString("O"),
                isPaused = frame.IsPaused,
                simulationSpeed = frame.SimulationSpeed,
                inputs = frame.Inputs,
                outputs = frame.Outputs,
            };

            var json = JsonConvert.SerializeObject(packet);
            sender.Send(Encoding.UTF8.GetBytes(json));
        }

        public void Dispose()
        {
            sender.Dispose();
        }
    }
}
