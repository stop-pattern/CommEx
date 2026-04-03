using System.Collections.Generic;

namespace CommEx.Models.Settings
{
    /// <summary>
    /// UDP 機能の設定値を保持します。
    /// </summary>
    internal class UdpSettings
    {
        /// <summary>
        /// UDP 機能の有効状態を取得または設定します。
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// NIC 単位の設定一覧を取得または設定します。
        /// </summary>
        public IList<UdpNicSettings> Nics { get; set; } = new List<UdpNicSettings>
        {
            new UdpNicSettings
            {
                NicName = "Default",
                LocalIpAddress = "192.168.0.10",
                DestinationIpAddress = "192.168.0.100"
            }
        };
    }
}
