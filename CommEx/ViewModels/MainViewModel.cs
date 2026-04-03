using System;
using System.Collections.Generic;
using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Models;
using CommEx.Services;

namespace CommEx.ViewModels
{
    /// <summary>
    /// プラグインのフレーム更新処理と有効/無効状態を管理する ViewModel です。
    /// </summary>
    internal class MainViewModel : IDisposable
    {
        private readonly ITelemetryService telemetryService;
        private readonly IComCommunicationService comCommunicationService;
        private readonly IPluginLogger logger;
        private readonly IClock clock;
        private readonly PluginState state = new PluginState();

        /// <summary>
        /// <see cref="MainViewModel"/> の新しいインスタンスを初期化します。
        /// </summary>
        public MainViewModel(
            ITelemetryService telemetryService,
            IComCommunicationService comCommunicationService,
            IClock clock,
            IPluginLogger logger)
        {
            this.telemetryService = telemetryService;
            this.comCommunicationService = comCommunicationService;
            this.clock = clock;
            this.logger = logger;

            ComTransportModel model = new ComTransportModel(new List<ComPortConfiguration>
            {
                new ComPortConfiguration
                {
                    PortName = "COM3",
                    ProtocolType = ComProtocolType.Bids,
                },
            });

            comCommunicationService.Start(model);
        }

        /// <summary>
        /// プラグインの有効状態を取得または設定します。
        /// </summary>
        public bool IsEnabled
        {
            get { return state.IsEnabled; }
            set { state.IsEnabled = value; }
        }

        /// <summary>
        /// 毎フレーム呼び出される更新処理を実行します。
        /// </summary>
        /// <param name="elapsed">前フレームからの経過時間。</param>
        public void OnTick(TimeSpan elapsed)
        {
            if (!state.IsEnabled)
            {
                return;
            }

            state.TickCount++;
            state.LastTickUtc = clock.UtcNow;

            TelemetrySnapshot snapshot = new TelemetrySnapshot
            {
                TickCount = state.TickCount,
                TickUtc = state.LastTickUtc,
            };

            telemetryService.PublishHeartbeat(state.TickCount, elapsed);
            comCommunicationService.PublishSnapshot(snapshot);

            if (state.TickCount % 600 == 0)
            {
                logger.Info("CommEx heartbeat is active.");
            }
        }

        public void Dispose()
        {
            comCommunicationService.Stop();
        }
    }
}
