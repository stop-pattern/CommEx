using System;
using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;

namespace CommEx.Services
{
    internal class DummyTelemetryService : ITelemetryService
    {
        private readonly IPluginLogger logger;
        private readonly IClock clock;

        public DummyTelemetryService(IPluginLogger logger, IClock clock)
        {
            this.logger = logger;
            this.clock = clock;
        }

        public void PublishHeartbeat(int tickCount, TimeSpan elapsed)
        {
            if (tickCount == 1)
            {
                logger.Info("Telemetry service initialized at " + clock.UtcNow.ToString("O"));
            }

            _ = elapsed;
        }
    }
}
