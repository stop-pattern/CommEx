namespace CommEx.Infrastructure.Logging
{
    internal class NullPluginLogger : IPluginLogger
    {
        public void Info(string message)
        {
            _ = message;
        }
    }
}
