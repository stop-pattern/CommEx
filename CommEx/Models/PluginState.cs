using System;

namespace CommEx.Models
{
    /// <summary>
    /// プラグイン実行中の状態を保持するモデルです。
    /// </summary>
    internal class PluginState
    {
        /// <summary>
        /// プラグイン機能の有効状態を取得または設定します。
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 最後に Tick 処理を実行した UTC 時刻を取得または設定します。
        /// </summary>
        public DateTime LastTickUtc { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Tick が実行された回数を取得または設定します。
        /// </summary>
        public int TickCount { get; set; }
    }
}
