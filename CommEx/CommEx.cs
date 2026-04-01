using System;
using System.Collections.Generic;

using BveEx.PluginHost.Plugins;
using BveEx.PluginHost.Plugins.Extensions;

using CommEx.Infrastructure;
using CommEx.Model;

namespace CommEx
{
    [Plugin(PluginType.Extension)]
    [Togglable]
    internal class ExtensionMain : AssemblyPluginBase, ITogglableExtension, IExtension
    {
        private bool status = true;
        private readonly BveExModelStore modelStore;
        private readonly ApiBridgeService apiBridge;
        private readonly NtpServerService ntpServer;
        private readonly CommunicationWorker communicationWorker;
        private readonly BidsSerialCommunicationService bidsService;

        public bool IsEnabled
        {
            get { return status; }
            set { status = value; }
        }

        public ExtensionMain(PluginBuilder builder) : base(builder)
        {
            modelStore = new BveExModelStore();

            var publishers = new List<IFramePublisher>
            {
                new UdpRelayService(new UdpSender("127.0.0.1", 19100)),

                // TODO: 実際のポート設定を追加するまで NullSender で無効化
                new SerialRelayService(new NullSerialSender(), SerialProtocol.Bids),
                new SerialRelayService(new NullSerialSender(), SerialProtocol.Communication),
                new SerialRelayService(new NullSerialSender(), SerialProtocol.Binary),
            };

            communicationWorker = new CommunicationWorker(publishers);
            apiBridge = new ApiBridgeService(modelStore, "http://127.0.0.1:19101/");
            ntpServer = new NtpServerService(modelStore, 19123);
            bidsService = new BidsSerialCommunicationService(
                new NullBidsSerialPort(),
                modelStore,
                new BidsRequestParser(),
                new BidsResponseValueGenerator(),
                new BidsResponseFormatter());

            communicationWorker.Start();
            apiBridge.Start();
            ntpServer.Start();
            bidsService.Start();
        }

        public override void Dispose()
        {
            bidsService.Dispose();
            ntpServer.Dispose();
            apiBridge.Dispose();
            communicationWorker.Dispose();
        }

        public override void Tick(TimeSpan elapsed)
        {
            if (!status)
            {
                return;
            }

            var frame = CaptureFromBveEx();
            modelStore.Update(frame, DateTimeOffset.UtcNow);
            communicationWorker.EnqueueLatest(frame);
        }

        private SimulationFrame CaptureFromBveEx()
        {
            // TODO: BveEx API 連携実装。
            var inputs = new Dictionary<string, object>
            {
                { "brakeNotch", 0 },
                { "powerNotch", 0 },
                { "reverser", 0 },
            };

            var outputs = new Dictionary<string, object>
            {
                { "speedKmph", 0.0 },
                { "locationM", 0.0 },
                { "bcPressure", 0.0 },
            };

            return new SimulationFrame(
                DateTimeOffset.UtcNow,
                false,
                1.0,
                inputs,
                outputs);
        }
    }
}
