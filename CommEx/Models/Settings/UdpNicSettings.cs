namespace CommEx.Models.Settings
{
    /// <summary>
    /// UDP 通信における NIC ごとの設定値です。
    /// </summary>
    internal class UdpNicSettings
    {
        /// <summary>
        /// NIC 名を取得または設定します。
        /// </summary>
        public string NicName { get; set; } = string.Empty;

        /// <summary>
        /// NIC のローカル IP を取得または設定します。
        /// </summary>
        public string LocalIpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 送信先 IP を取得または設定します。
        /// </summary>
        public string DestinationIpAddress { get; set; } = string.Empty;
    }
}
