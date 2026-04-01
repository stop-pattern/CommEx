using System;
using System.Collections.Generic;

using BveEx.PluginHost.Plugins;
using BveEx.PluginHost.Plugins.Extensions;

using BveExCsTemplate.Extension.Infrastructure;
using BveExCsTemplate.Extension.Model;

namespace BveExCsTemplate.Extension
{
    [Plugin(PluginType.Extension)]
    [Togglable]
    internal class ExtensionMain : AssemblyPluginBase, ITogglableExtension, IExtension
    {
        private bool status = true;
        private readonly BveExModelStore modelStore;
        private readonly UdpRelayService udpRelay;
        private readonly ApiBridgeService apiBridge;
        private readonly NtpServerService ntpServer;

        public bool IsEnabled
        {
            get { return status; }
            set { status = value; }
        }

        public ExtensionMain(PluginBuilder builder) : base(builder)
        {
            modelStore = new BveExModelStore();

            // TODO: 設定ファイルから読み込む
            udpRelay = new UdpRelayService(new UdpSender("127.0.0.1", 19100));
            apiBridge = new ApiBridgeService(modelStore, "http://127.0.0.1:19101/");
            ntpServer = new NtpServerService(modelStore, 19123);

            apiBridge.Start();
            ntpServer.Start();
        }

        public override void Dispose()
        {
            ntpServer.Dispose();
            apiBridge.Dispose();
            udpRelay.Dispose();
        }

        public override void Tick(TimeSpan elapsed)
        {
            if (!status)
            {
                return;
            }

            var frame = CaptureFromBveEx();
            modelStore.Update(frame, DateTimeOffset.UtcNow);
            udpRelay.Publish(frame);
        }

        private SimulationFrame CaptureFromBveEx()
        {
            // TODO: BveEx API 連携実装。
            // 現状は「そのまま垂れ流す」ための仮ペイロード。
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
