using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Services;
using CommEx.ViewModels;

namespace CommEx.App
{
    internal static class PluginCompositionRoot
    {
        public static MainViewModel BuildMainViewModel()
        {
            IPluginLogger logger = new NullPluginLogger();
            IClock clock = new SystemClock();
            ITelemetryService telemetryService = new DummyTelemetryService(logger, clock);

            return new MainViewModel(telemetryService, logger);
        }
    }
}
