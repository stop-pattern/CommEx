namespace CommEx.Infrastructure.Logging
{
    /// <summary>
    /// ログを実際には出力しない Null Object パターンのロガーです。
    /// </summary>
    internal class NullPluginLogger : IPluginLogger
    {
        /// <summary>
        /// ログメッセージを受け取りますが、何も出力しません。
        /// </summary>
        /// <param name="message">出力要求されたメッセージ。</param>
        public void Info(string message)
        {
            _ = message;
        }
    }
}
