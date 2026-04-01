using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

using CommEx.Model;

namespace CommEx.Infrastructure
{
    internal sealed class BidsRequest
    {
        public BidsRequest(string header, char kind, string requestCode)
        {
            Header = header;
            Kind = kind;
            RequestCode = requestCode;
        }

        public string Header { get; }
        public char Kind { get; }
        public string RequestCode { get; }
    }

    internal sealed class BidsRequestParser
    {
        private static readonly Regex RequestRegex = new Regex("^(EX|TR)([A-Za-z0-9])([A-Za-z0-9]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryParse(string raw, out BidsRequest request)
        {
            request = null;
            if (string.IsNullOrWhiteSpace(raw)) return false;

            var match = RequestRegex.Match(raw.Trim());
            if (!match.Success) return false;

            var header = match.Groups[1].Value.ToUpperInvariant();
            var kind = char.ToUpperInvariant(match.Groups[2].Value[0]);
            var code = match.Groups[3].Value.ToUpperInvariant();
            request = new BidsRequest(header, kind, code);
            return true;
        }
    }

    internal sealed class BidsResponseValueGenerator
    {
        public object Generate(BidsRequest request, SimulationFrame frame)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            switch (request.RequestCode)
            {
                case "SPD":
                    return GetDouble(frame.Outputs, "speedKmph");
                case "LOC":
                    return GetDouble(frame.Outputs, "locationM");
                case "BRK":
                    return GetInt(frame.Inputs, "brakeNotch");
                case "PWR":
                    return GetInt(frame.Inputs, "powerNotch");
                case "PAU":
                    return frame.IsPaused;
                case "SIM":
                    return frame.SimulationSpeed;
                default:
                    return 0;
            }
        }

        private static double GetDouble(IReadOnlyDictionary<string, object> source, string key)
        {
            object value;
            return source.TryGetValue(key, out value) ? Convert.ToDouble(value, CultureInfo.InvariantCulture) : 0d;
        }

        private static int GetInt(IReadOnlyDictionary<string, object> source, string key)
        {
            object value;
            return source.TryGetValue(key, out value) ? Convert.ToInt32(value, CultureInfo.InvariantCulture) : 0;
        }
    }

    internal sealed class BidsResponseFormatter
    {
        public string Format(BidsRequest request, object value)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var text = SerializeValue(value);
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}X{3}", request.Header, request.Kind, request.RequestCode, text);
        }

        private static string SerializeValue(object value)
        {
            if (value == null) return "0";

            if (value is bool)
            {
                return ((bool)value) ? "true" : "false";
            }

            if (value is double)
            {
                return ((double)value).ToString("0.########", CultureInfo.InvariantCulture);
            }

            if (value is float)
            {
                return ((float)value).ToString("0.######", CultureInfo.InvariantCulture);
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    internal interface IBidsSerialPort : IDisposable
    {
        bool TryRead(out string line);
        void Write(string line);
    }

    internal sealed class NullBidsSerialPort : IBidsSerialPort
    {
        public bool TryRead(out string line)
        {
            line = null;
            return false;
        }

        public void Write(string line)
        {
        }

        public void Dispose()
        {
        }
    }

    internal sealed class BidsSerialCommunicationService : IDisposable
    {
        private readonly IBidsSerialPort port;
        private readonly BveExModelStore modelStore;
        private readonly BidsRequestParser parser;
        private readonly BidsResponseValueGenerator valueGenerator;
        private readonly BidsResponseFormatter formatter;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Thread thread;

        public BidsSerialCommunicationService(
            IBidsSerialPort port,
            BveExModelStore modelStore,
            BidsRequestParser parser,
            BidsResponseValueGenerator valueGenerator,
            BidsResponseFormatter formatter)
        {
            this.port = port;
            this.modelStore = modelStore;
            this.parser = parser;
            this.valueGenerator = valueGenerator;
            this.formatter = formatter;
            thread = new Thread(Loop)
            {
                IsBackground = true,
                Name = "CommEx.BidsSerialService",
            };
        }

        public void Start()
        {
            thread.Start();
        }

        public bool ProcessOnce()
        {
            string line;
            if (!port.TryRead(out line))
            {
                return false;
            }

            BidsRequest request;
            if (!parser.TryParse(line, out request))
            {
                return false;
            }

            var frame = modelStore.GetCurrentFrame();
            var value = valueGenerator.Generate(request, frame);
            var response = formatter.Format(request, value);
            port.Write(response);
            return true;
        }

        private void Loop()
        {
            while (!cts.IsCancellationRequested)
            {
                if (!ProcessOnce())
                {
                    Thread.Sleep(5);
                }
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            if (thread.IsAlive)
            {
                thread.Join(TimeSpan.FromSeconds(1));
            }
            port.Dispose();
            cts.Dispose();
        }
    }
}
