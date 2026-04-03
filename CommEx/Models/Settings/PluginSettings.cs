using System.Collections.Generic;

namespace CommEx.Models.Settings
{
    /// <summary>
    /// 右クリックメニューから編集するプラグイン設定を保持します。
    /// </summary>
    internal class PluginSettings
    {
        /// <summary>
        /// UDP 通信設定を取得または設定します。
        /// </summary>
        public UdpSettings Udp { get; set; } = new UdpSettings();

        /// <summary>
        /// API サーバー設定を取得または設定します。
        /// </summary>
        public ApiServerSettings ApiServer { get; set; } = new ApiServerSettings();

        /// <summary>
        /// COM ポート設定一覧を取得または設定します。
        /// </summary>
        public IList<ComPortSettings> ComPorts { get; set; } = new List<ComPortSettings>
        {
            new ComPortSettings()
        };
    }
}
