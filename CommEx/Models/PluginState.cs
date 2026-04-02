using System;

namespace CommEx.Models
{
    internal class PluginState
    {
        public bool IsEnabled { get; set; } = true;

        public DateTime LastTickUtc { get; set; } = DateTime.MinValue;

        public int TickCount { get; set; }
    }
}
