namespace CommEx.Models.Settings
{
    /// <summary>
    /// API サーバー機能の設定値を保持します。
    /// </summary>
    internal class ApiServerSettings
    {
        /// <summary>
        /// API サーバー機能の有効状態を取得または設定します。
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// API サーバーの待受ポート番号を取得または設定します。
        /// </summary>
        public int Port { get; set; } = 31000;
    }
}
