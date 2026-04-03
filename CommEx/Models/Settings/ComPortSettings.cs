namespace CommEx.Models.Settings
{
    /// <summary>
    /// COM ポート単位の設定値です。
    /// </summary>
    internal class ComPortSettings
    {
        /// <summary>
        /// 当該 COM ポート設定の有効状態を取得または設定します。
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 使用するプロトコルを取得または設定します。
        /// </summary>
        public ComProtocolType Protocol { get; set; } = ComProtocolType.BidsCompatible;

        /// <summary>
        /// COM ポート名を取得または設定します。
        /// </summary>
        public string PortName { get; set; } = "COM1";

        /// <summary>
        /// ボーレートを取得または設定します。
        /// </summary>
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// データビット数を取得または設定します。
        /// </summary>
        public int DataBits { get; set; } = 8;

        /// <summary>
        /// ストップビット数を取得または設定します。
        /// </summary>
        public int StopBits { get; set; } = 1;

        /// <summary>
        /// パリティ設定値を取得または設定します。
        /// </summary>
        public string Parity { get; set; } = "None";

        /// <summary>
        /// DTR の有効状態を取得または設定します。
        /// </summary>
        public bool IsDtrEnabled { get; set; }

        /// <summary>
        /// RTS の有効状態を取得または設定します。
        /// </summary>
        public bool IsRtsEnabled { get; set; }

        /// <summary>
        /// 自動起動設定を取得または設定します。
        /// </summary>
        public bool AutoStart { get; set; }
    }
}
