using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;

using CommEx.Model;

using Newtonsoft.Json;

namespace CommEx.Infrastructure
{
    internal enum SerialProtocol
    {
        Bids,
        Communication,
        Binary,
    }

    internal interface ISerialSender : IDisposable
    {
        void Send(byte[] payload);
    }

    internal sealed class SerialPortSender : ISerialSender
    {
        private readonly SerialPort port;

        public SerialPortSender(string portName, int baudRate)
        {
            port = new SerialPort(portName, baudRate);
            port.Open();
        }

        public void Send(byte[] payload)
        {
            port.Write(payload, 0, payload.Length);
        }

        public void Dispose()
        {
            port.Dispose();
        }
    }

    internal sealed class NullSerialSender : ISerialSender
    {
        public void Send(byte[] payload)
        {
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// BveEx 情報を各種シリアルプロトコルへ変換して出力する。
    /// </summary>
    internal sealed class SerialRelayService : IFramePublisher, IDisposable
    {
        private readonly ISerialSender sender;
        private readonly SerialProtocol protocol;

        public SerialRelayService(ISerialSender sender, SerialProtocol protocol)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.protocol = protocol;
        }

        public void Publish(SimulationFrame frame)
        {
            sender.Send(Encode(frame));
        }

        internal byte[] Encode(SimulationFrame frame)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            switch (protocol)
            {
                case SerialProtocol.Bids:
                    return EncodeBids(frame);
                case SerialProtocol.Communication:
                    return EncodeCommunication(frame);
                case SerialProtocol.Binary:
                    return EncodeBinary(frame);
                default:
                    throw new InvalidOperationException("Unknown protocol.");
            }
        }

        private static byte[] EncodeBids(SimulationFrame frame)
        {
            var speed = GetDouble(frame.Outputs, "speedKmph");
            var brake = GetInt(frame.Inputs, "brakeNotch");
            var line = string.Format("BIDS;TIME={0:O};SPD={1:0.00};BRK={2}\r\n", frame.BveTimeUtc.UtcDateTime, speed, brake);
            return Encoding.ASCII.GetBytes(line);
        }

        private static byte[] EncodeCommunication(SimulationFrame frame)
        {
            var payload = new Dictionary<string, object>
            {
                { "protocol", "communication" },
                { "time", frame.BveTimeUtc.UtcDateTime.ToString("O") },
                { "inputs", frame.Inputs },
                { "outputs", frame.Outputs },
            };
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload) + "\n");
        }

        private static byte[] EncodeBinary(SimulationFrame frame)
        {
            var speed = (float)GetDouble(frame.Outputs, "speedKmph");
            var brake = (short)GetInt(frame.Inputs, "brakeNotch");
            var power = (short)GetInt(frame.Inputs, "powerNotch");
            var unixMillis = frame.BveTimeUtc.ToUnixTimeMilliseconds();

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(new[] { (byte)'C', (byte)'M', (byte)'E', (byte)'X' });
                bw.Write(unixMillis);
                bw.Write(speed);
                bw.Write(brake);
                bw.Write(power);
                bw.Flush();
                return ms.ToArray();
            }
        }

        private static double GetDouble(IReadOnlyDictionary<string, object> dict, string key)
        {
            object value;
            return dict.TryGetValue(key, out value) ? Convert.ToDouble(value) : 0d;
        }

        private static int GetInt(IReadOnlyDictionary<string, object> dict, string key)
        {
            object value;
            return dict.TryGetValue(key, out value) ? Convert.ToInt32(value) : 0;
        }

        public void Dispose()
        {
            sender.Dispose();
        }
    }
}
