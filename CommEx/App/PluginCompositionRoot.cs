using System.Collections.Generic;
using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Services;
using CommEx.Services.Settings;
using CommEx.ViewModels;
using CommEx.ViewModels.Settings;
using CommEx.Views.Settings;

namespace CommEx.App
{
    /// <summary>
    /// MVVM の依存関係を組み立てる Composition Root です。
    /// </summary>
    internal static class PluginCompositionRoot
    {
        /// <summary>
        /// プラグイン起動時に使用するメイン ViewModel を生成します。
        /// </summary>
        /// <returns>依存関係を注入済みの <see cref="MainViewModel"/>。</returns>
        public static MainViewModel BuildMainViewModel()
        {
            IPluginLogger logger = new NullPluginLogger();
            IClock clock = new SystemClock();
            ITelemetryService telemetryService = new DummyTelemetryService(logger, clock);

            IPluginSettingsRepository settingsRepository = new InMemoryPluginSettingsRepository();
            IPluginSettingsService settingsService = new PluginSettingsService(settingsRepository);

            IEnumerable<ISettingsSectionViewModelFactory> factories = new ISettingsSectionViewModelFactory[]
            {
                new UdpSettingsSectionViewModelFactory(),
                new ApiServerSettingsSectionViewModelFactory(),
                new ComSettingsSectionViewModelFactory()
            };

            ContextMenuSettingsViewModel settingsViewModel = new ContextMenuSettingsViewModel(settingsService, factories);
            IContextMenuSettingsView settingsView = new DeferredContextMenuSettingsView(logger);

            return new MainViewModel(telemetryService, logger, clock, settingsView, settingsViewModel);
        }
    }
}
