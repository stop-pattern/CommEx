using System;
using System.Net;
using System.Net.Sockets;

namespace BveExCsTemplate.Extension.Infrastructure
{
    internal sealed class UdpSender : IUdpSender
    {
        private readonly UdpClient client;
        private readonly IPEndPoint destination;

        public UdpSender(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("Host is required.", nameof(host));
            client = new UdpClient();
            destination = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public void Send(byte[] packet)
        {
            client.Send(packet, packet.Length, destination);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
