using System;

namespace CommEx.Infrastructure.Time
{
    internal interface IClock
    {
        DateTime UtcNow { get; }
    }
}
