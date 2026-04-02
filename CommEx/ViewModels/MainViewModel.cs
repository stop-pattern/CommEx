using System;
using CommEx.Infrastructure.Logging;
using CommEx.Models;
using CommEx.Services;

namespace CommEx.ViewModels
{
    internal class MainViewModel
    {
        private readonly ITelemetryService telemetryService;
        private readonly IPluginLogger logger;
        private readonly PluginState state = new PluginState();

        public MainViewModel(ITelemetryService telemetryService, IPluginLogger logger)
        {
            this.telemetryService = telemetryService;
            this.logger = logger;
        }

        public bool IsEnabled
        {
            get { return state.IsEnabled; }
            set { state.IsEnabled = value; }
        }

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
