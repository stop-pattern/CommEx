using System;

namespace CommEx.Infrastructure.Time
{
    /// <summary>
    /// 現在時刻取得を抽象化するインターフェースです。
    /// </summary>
    internal interface IClock
    {
        /// <summary>
        /// 現在の UTC 時刻を取得します。
        /// </summary>
        DateTime UtcNow { get; }
    }
}
