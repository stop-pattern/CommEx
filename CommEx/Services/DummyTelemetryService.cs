using System;
using CommEx.Infrastructure.Logging;
using CommEx.Infrastructure.Time;
using CommEx.Models;

namespace CommEx.Services
{
    /// <summary>
    /// 開発初期の疎通確認用テレメトリサービスです。
    /// </summary>
    internal class DummyTelemetryService : ITelemetryService
    {
        private readonly IPluginLogger logger;
        private readonly IClock clock;

        /// <summary>
        /// <see cref="DummyTelemetryService"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logger">ログ出力先。</param>
        /// <param name="clock">現在時刻の取得元。</param>
        public DummyTelemetryService(IPluginLogger logger, IClock clock)
        {
            this.logger = logger;
            this.clock = clock;
        }

        /// <summary>
        /// 初回呼び出し時に初期化ログを出力し、テレメトリ連携の導線を確認します。
        /// </summary>
        /// <param name="tickCount">Tick 呼び出し回数。</param>
        /// <param name="elapsed">前フレームからの経過時間。</param>
        public void PublishHeartbeat(int tickCount, TimeSpan elapsed)
        {
            if (tickCount == 1)
            {
                logger.Info("Telemetry service initialized at " + clock.UtcNow.ToString("O"));
            }

            _ = elapsed;
        }

        public void PublishUdpTelemetry(UdpTelemetryPacket packet)
        {
            _ = packet;
        }

        public void Dispose()
        {
        }
    }
}
