using System;

namespace CommEx.Services
{
    /// <summary>
    /// プラグインの状態を外部へ通知するテレメトリサービスの契約です。
    /// </summary>
    internal interface ITelemetryService
    {
        /// <summary>
        /// ハートビート情報を通知します。
        /// </summary>
        /// <param name="tickCount">Tick 呼び出し回数。</param>
        /// <param name="elapsed">前フレームからの経過時間。</param>
        void PublishHeartbeat(int tickCount, TimeSpan elapsed);
    }
}
