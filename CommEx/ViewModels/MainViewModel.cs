using System;
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
        private readonly IPluginLogger logger;
        private readonly IClock clock;
        private readonly PluginState state = new PluginState();

        /// <summary>
        /// <see cref="MainViewModel"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="telemetryService">テレメトリ通知を担当するサービス。</param>
        /// <param name="logger">ログ出力を担当するロガー。</param>
        /// <param name="clock">現在時刻の取得元。</param>
        public MainViewModel(ITelemetryService telemetryService, IPluginLogger logger, IClock clock)
        {
            this.telemetryService = telemetryService;
            this.logger = logger;
            this.clock = clock;
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

            telemetryService.PublishHeartbeat(state.TickCount, elapsed);
            telemetryService.PublishUdpTelemetry(CreateSamplePacket(state.TickCount));

            if (state.TickCount % 600 == 0)
            {
                logger.Info("CommEx heartbeat is active.");
            }
        }

        public void Dispose()
        {
            telemetryService.Dispose();
        }

        private static UdpTelemetryPacket CreateSamplePacket(int tickCount)
        {
            double currentPosition = tickCount * 0.5;
            double currentSpeed = Math.Min(95.0, tickCount % 120);
            bool isDoorOpen = tickCount % 120 < 10;
            int handlePosition = (tickCount / 30) % 6;

            return new UdpTelemetryPacket(currentPosition, currentSpeed, isDoorOpen, handlePosition);
        }
    }
}
