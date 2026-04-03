using System;

namespace CommEx.Infrastructure.Time
{
    /// <summary>
    /// システム時刻を返す <see cref="IClock"/> 実装です。
    /// </summary>
    internal class SystemClock : IClock
    {
        /// <summary>
        /// 現在の UTC 時刻を取得します。
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
