using System.Globalization;

namespace CommEx.Models
{
    /// <summary>
    /// UDP で送信するテレメトリのドメインモデルです。
    /// </summary>
    internal class UdpTelemetryPacket
    {
        public UdpTelemetryPacket(double currentPosition, double currentSpeed, bool isDoorOpen, int handlePosition)
        {
            CurrentPosition = currentPosition;
            CurrentSpeed = currentSpeed;
            IsDoorOpen = isDoorOpen;
            HandlePosition = handlePosition;
        }

        /// <summary>現在位置。</summary>
        public double CurrentPosition { get; }

        /// <summary>現在速度。</summary>
        public double CurrentSpeed { get; }

        /// <summary>ドア開閉状態。true で開。</summary>
        public bool IsDoorOpen { get; }

        /// <summary>ハンドル位置。</summary>
        public int HandlePosition { get; }

        /// <summary>
        /// 暫定 CSV ペイロードへ変換します。
        /// 形式: 現在位置,現在速度,ドア開閉状態,ハンドル位置
        /// </summary>
        /// <returns>UDP 送信用の文字列。</returns>
        public string ToCsvPayload()
        {
            string doorState = IsDoorOpen ? "open" : "close";

            return string.Join(",",
                CurrentPosition.ToString("F1", CultureInfo.InvariantCulture),
                CurrentSpeed.ToString("F1", CultureInfo.InvariantCulture),
                doorState,
                HandlePosition.ToString(CultureInfo.InvariantCulture));
        }
    }
}
