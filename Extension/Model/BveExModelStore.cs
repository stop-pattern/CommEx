using System;
using System.Collections.Generic;

namespace BveExCsTemplate.Extension.Model
{
    /// <summary>
    /// MVVM の Model として BveEx の最新状態を保持する。
    /// </summary>
    internal sealed class BveExModelStore
    {
        private readonly object gate = new object();

        private SimulationFrame latestFrame = new SimulationFrame(
            new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero),
            true,
            1d,
            new Dictionary<string, object>(),
            new Dictionary<string, object>());

        private DateTimeOffset updatedAtUtc = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public void Update(SimulationFrame frame, DateTimeOffset updatedAt)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            lock (gate)
            {
                latestFrame = frame;
                updatedAtUtc = updatedAt;
            }
        }

        public SimulationFrame GetCurrentFrame()
        {
            lock (gate)
            {
                return latestFrame;
            }
        }

        /// <summary>
        /// 一時停止・早送りを反映した現在時刻を返す。
        /// </summary>
        public DateTimeOffset GetVirtualNowUtc(DateTimeOffset nowUtc)
        {
            lock (gate)
            {
                if (latestFrame.IsPaused)
                {
                    return latestFrame.BveTimeUtc;
                }

                var elapsed = nowUtc - updatedAtUtc;
                if (elapsed < TimeSpan.Zero)
                {
                    elapsed = TimeSpan.Zero;
                }

                var scaled = TimeSpan.FromTicks((long)(elapsed.Ticks * latestFrame.SimulationSpeed));
                return latestFrame.BveTimeUtc + scaled;
            }
        }
    }
}
