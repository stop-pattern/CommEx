namespace CommEx.Infrastructure.Logging
{
    /// <summary>
    /// プラグイン内部で利用するログ出力の抽象化です。
    /// </summary>
    internal interface IPluginLogger
    {
        /// <summary>
        /// 情報レベルのログを出力します。
        /// </summary>
        /// <param name="message">出力するログメッセージ。</param>
        void Info(string message);
    }
}
