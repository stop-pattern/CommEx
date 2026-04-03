namespace CommEx.Models
{
    /// <summary>
    /// UDP 通信に関する設定値モデルです。
    /// </summary>
    internal class UdpTransportSettings
    {
        /// <summary>有効フラグ。</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>宛先 IP。</summary>
        public string DestinationIp { get; set; } = "127.0.0.1";

        /// <summary>宛先ポート。</summary>
        public int DestinationPort { get; set; } = 50400;

        /// <summary>送信元ポート（0 で OS 自動割当）。</summary>
        public int LocalPort { get; set; }
    }
}
