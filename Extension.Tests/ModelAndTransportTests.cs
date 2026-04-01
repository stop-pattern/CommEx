using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using BveExCsTemplate.Extension.Infrastructure;
using BveExCsTemplate.Extension.Model;

using Newtonsoft.Json.Linq;

using Xunit;

namespace Extension.Tests
{
    public class ModelAndTransportTests
    {
        [Fact]
        public void VirtualClock_ReflectsPauseAndSpeed()
        {
            var store = new BveExModelStore();
            var baseTime = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            store.Update(new SimulationFrame(baseTime, false, 2.0, new Dictionary<string, object>(), new Dictionary<string, object>()), baseTime);
            var accelerated = store.GetVirtualNowUtc(baseTime.AddSeconds(3));
            Assert.Equal(baseTime.AddSeconds(6), accelerated);

            store.Update(new SimulationFrame(baseTime.AddSeconds(10), true, 8.0, new Dictionary<string, object>(), new Dictionary<string, object>()), baseTime.AddSeconds(10));
            var paused = store.GetVirtualNowUtc(baseTime.AddSeconds(50));
            Assert.Equal(baseTime.AddSeconds(10), paused);
        }

        [Fact]
        public void UdpRelay_PublishesRawFrameAsJson()
        {
            var sender = new FakeUdpSender();
            var relay = new UdpRelayService(sender);
            var frame = new SimulationFrame(
                new DateTimeOffset(2026, 2, 3, 4, 5, 6, TimeSpan.Zero),
                false,
                1.5,
                new Dictionary<string, object> { { "x", 1 } },
                new Dictionary<string, object> { { "y", 2 } });

            relay.Publish(frame);

            var json = Encoding.UTF8.GetString(sender.LastPacket);
            var root = JObject.Parse(json);
            Assert.Equal("bveex.raw", (string)root["type"]);
            Assert.Equal(1, (int)root["inputs"]["x"]);
            Assert.Equal(2, (int)root["outputs"]["y"]);
        }

        [Fact]
        public void NtpCodec_ContainsTransmitTimestamp()
        {
            var now = new DateTimeOffset(2026, 3, 31, 12, 0, 0, TimeSpan.Zero);
            var response = NtpPacketCodec.CreateServerResponse(new byte[48], now);
            var decoded = NtpPacketCodec.ReadTimestamp(response, 40);

            Assert.InRange((decoded - now).Duration(), TimeSpan.Zero, TimeSpan.FromMilliseconds(2));
        }

        [Fact]
        public void ApiBridge_BuildValuePayload_ReturnsValueAndNull()
        {
            var service = new ApiBridgeService(new BveExModelStore(), "http://127.0.0.1:29101/");
            var source = new Dictionary<string, object> { { "speed", 80 } };

            var found = service.BuildValuePayload(source, "speed", "output");
            var notFound = service.BuildValuePayload(source, "unknown", "output");

            Assert.Equal(80, (int)JObject.FromObject(found)["value"]);
            Assert.Equal(JTokenType.Null, JObject.FromObject(notFound)["value"].Type);

            service.Dispose();
        }


        [Fact]
        public void SerialRelay_EncodesAllThreeProtocols()
        {
            var frame = new SimulationFrame(
                new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                false,
                1.0,
                new Dictionary<string, object> { { "brakeNotch", 3 }, { "powerNotch", 2 } },
                new Dictionary<string, object> { { "speedKmph", 45.5 } });

            var bids = new SerialRelayService(new FakeSerialSender(), SerialProtocol.Bids).Encode(frame);
            Assert.Contains("BIDS;", Encoding.ASCII.GetString(bids));

            var comm = new SerialRelayService(new FakeSerialSender(), SerialProtocol.Communication).Encode(frame);
            Assert.Contains("\"protocol\":\"communication\"", Encoding.UTF8.GetString(comm));

            var bin = new SerialRelayService(new FakeSerialSender(), SerialProtocol.Binary).Encode(frame);
            Assert.Equal((byte)'C', bin[0]);
            Assert.Equal((byte)'M', bin[1]);
            Assert.Equal((byte)'E', bin[2]);
            Assert.Equal((byte)'X', bin[3]);
        }

        [Fact]
        public void CommunicationWorker_DropsOldFramesWhenQueueIsFull()
        {
            var slow = new SlowPublisher();
            var worker = new CommunicationWorker(new List<IFramePublisher> { slow }, capacity: 1);
            worker.Start();

            var baseTime = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
            for (var i = 0; i < 20; i++)
            {
                worker.EnqueueLatest(new SimulationFrame(baseTime.AddSeconds(i), false, 1.0, new Dictionary<string, object>(), new Dictionary<string, object>()));
            }

            Thread.Sleep(120);
            worker.Dispose();

            Assert.True(slow.Count > 0);
            Assert.True(slow.LastTime >= baseTime.AddSeconds(10));
        }


        [Fact]
        public void BidsParser_ParsesHeaderKindAndRequestCode()
        {
            var parser = new BidsRequestParser();
            BidsRequest request;

            var ok = parser.TryParse("tr9spd", out request);

            Assert.True(ok);
            Assert.Equal("TR", request.Header);
            Assert.Equal('9', request.Kind);
            Assert.Equal("SPD", request.RequestCode);
        }

        [Fact]
        public void BidsValueGenerator_GeneratesTypedValues()
        {
            var frame = new SimulationFrame(
                new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                true,
                2.5,
                new Dictionary<string, object> { { "brakeNotch", 5 }, { "powerNotch", 1 } },
                new Dictionary<string, object> { { "speedKmph", 83.25 }, { "locationM", 1000.0 } });

            var generator = new BidsResponseValueGenerator();
            Assert.Equal(83.25, (double)generator.Generate(new BidsRequest("EX", 'A', "SPD"), frame));
            Assert.Equal(5, (int)generator.Generate(new BidsRequest("EX", 'A', "BRK"), frame));
            Assert.True((bool)generator.Generate(new BidsRequest("EX", 'A', "PAU"), frame));
        }

        [Fact]
        public void BidsFormatter_BuildsOutgoingPacket()
        {
            var formatter = new BidsResponseFormatter();
            var text = formatter.Format(new BidsRequest("EX", 'N', "SPD"), 12.5);
            Assert.Equal("EXNSPDX12.5", text);
        }

        [Fact]
        public void BidsSerialService_ProcessesRequestAndWritesResponse()
        {
            var store = new BveExModelStore();
            store.Update(
                new SimulationFrame(
                    new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    false,
                    1.0,
                    new Dictionary<string, object> { { "brakeNotch", 2 }, { "powerNotch", 3 } },
                    new Dictionary<string, object> { { "speedKmph", 55.0 }, { "locationM", 200.0 } }),
                DateTimeOffset.UtcNow);

            var port = new FakeBidsPort("ex1spd");
            var service = new BidsSerialCommunicationService(
                port,
                store,
                new BidsRequestParser(),
                new BidsResponseValueGenerator(),
                new BidsResponseFormatter());

            var processed = service.ProcessOnce();

            Assert.True(processed);
            Assert.Equal("EX1SPDX55", port.Written[0]);
            service.Dispose();
        }

        private sealed class FakeUdpSender : IUdpSender
        {
            public byte[] LastPacket { get; private set; }

            public void Send(byte[] packet)
            {
                LastPacket = packet;
            }

            public void Dispose()
            {
            }
        }

        private sealed class FakeSerialSender : ISerialSender
        {
            public byte[] LastPayload { get; private set; }

            public void Send(byte[] payload)
            {
                LastPayload = payload;
            }

            public void Dispose()
            {
            }
        }

        private sealed class SlowPublisher : IFramePublisher
        {
            public int Count { get; private set; }
            public DateTimeOffset LastTime { get; private set; }

            public void Publish(SimulationFrame frame)
            {
                Count++;
                LastTime = frame.BveTimeUtc;
                Thread.Sleep(10);
            }
        }

        private sealed class FakeBidsPort : IBidsSerialPort
        {
            private readonly Queue<string> inputs;
            public List<string> Written { get; } = new List<string>();

            public FakeBidsPort(params string[] requests)
            {
                inputs = new Queue<string>(requests);
            }

            public bool TryRead(out string line)
            {
                if (inputs.Count == 0)
                {
                    line = null;
                    return false;
                }

                line = inputs.Dequeue();
                return true;
            }

            public void Write(string line)
            {
                Written.Add(line);
            }

            public void Dispose()
            {
            }
        }
    }
}
