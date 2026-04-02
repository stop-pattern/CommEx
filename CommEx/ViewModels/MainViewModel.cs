using System;
using CommEx.Infrastructure.Logging;
using CommEx.Models;
using CommEx.Services;

namespace CommEx.ViewModels
{
    /// <summary>
    /// プラグインのフレーム更新処理と有効/無効状態を管理する ViewModel です。
    /// </summary>
    internal class MainViewModel
    {
        private readonly ITelemetryService telemetryService;
        private readonly IPluginLogger logger;
        private readonly PluginState state = new PluginState();

        /// <summary>
        /// <see cref="MainViewModel"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="telemetryService">テレメトリ通知を担当するサービス。</param>
        /// <param name="logger">ログ出力を担当するロガー。</param>
        public MainViewModel(ITelemetryService telemetryService, IPluginLogger logger)
        {
            this.telemetryService = telemetryService;
            this.logger = logger;
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
            state.LastTickUtc = DateTime.UtcNow;

            telemetryService.PublishHeartbeat(state.TickCount, elapsed);

            if (state.TickCount % 600 == 0)
            {
                logger.Info("CommEx heartbeat is active.");
            }
        }
    }
}
