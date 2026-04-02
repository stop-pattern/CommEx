using System;

namespace CommEx.Infrastructure.Time
{
    internal class SystemClock : IClock
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
