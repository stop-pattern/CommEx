using System;
using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Models;
using CommEx.Services;
using CommEx.ViewModels.Settings;
using CommEx.Views.Settings;

namespace CommEx.ViewModels
{
    /// <summary>
    /// プラグインのフレーム更新処理と有効/無効状態を管理する ViewModel です。
    /// </summary>
    internal class MainViewModel
    {
        private readonly ITelemetryService telemetryService;
        private readonly IPluginLogger logger;
        private readonly IClock clock;
        private readonly IContextMenuSettingsView settingsView;
        private readonly PluginState state = new PluginState();

        /// <summary>
        /// <see cref="MainViewModel"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="telemetryService">テレメトリ通知を担当するサービス。</param>
        /// <param name="logger">ログ出力を担当するロガー。</param>
        /// <param name="settingsView">右クリック設定表示を担当する View アダプタ。</param>
        /// <param name="settingsViewModel">右クリック設定画面の ViewModel。</param>
        public MainViewModel(
            ITelemetryService telemetryService,
            IPluginLogger logger,
            IClock clock,
            IContextMenuSettingsView settingsView,
            ContextMenuSettingsViewModel settingsViewModel)
        {
            this.telemetryService = telemetryService;
            this.logger = logger;
            this.clock = clock;
            this.settingsView = settingsView;
            SettingsViewModel = settingsViewModel;
        }

        /// <summary>
        /// 右クリック設定画面の ViewModel を取得します。
        /// </summary>
        public ContextMenuSettingsViewModel SettingsViewModel { get; }

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

            if (state.TickCount % 600 == 0)
            {
                logger.Info("CommEx heartbeat is active.");
            }
        }

        /// <summary>
        /// 右クリックメニューから設定表示が要求された際に呼び出します。
        /// </summary>
        public void OpenContextMenuSettings()
        {
            settingsView.Show(SettingsViewModel);
        }

        /// <summary>
        /// 設定の保存を実行します。
        /// </summary>
        public void SaveSettings()
        {
            SettingsViewModel.Save();
        }
    }
}
