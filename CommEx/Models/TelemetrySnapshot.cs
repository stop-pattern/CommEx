using System;

namespace CommEx.Models
{
    /// <summary>
    /// フレーム単位で通信へ渡すスナップショットです。
    /// </summary>
    internal sealed class TelemetrySnapshot
    {
        public int TickCount { get; set; }

        public DateTime TickUtc { get; set; }
    }
}
