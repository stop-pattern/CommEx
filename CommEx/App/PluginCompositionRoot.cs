using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Models;
using CommEx.Services;
using CommEx.ViewModels;

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
            UdpTransportSettings udpSettings = new UdpTransportSettings();
            ITelemetryService telemetryService = new UdpTelemetryService(logger, clock, udpSettings);

            return new MainViewModel(telemetryService, logger, clock);
        }
    }
}
