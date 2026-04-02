using System;

namespace CommEx.Services
{
    internal interface ITelemetryService
    {
        void PublishHeartbeat(int tickCount, TimeSpan elapsed);
    }
}
