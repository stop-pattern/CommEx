using CommEx.Models.Settings;

namespace CommEx.Services.Settings
{
    /// <summary>
    /// メモリ上に設定を保持する簡易実装です。
    /// UI 実装前の暫定ストレージとして利用します。
    /// </summary>
    internal class InMemoryPluginSettingsRepository : IPluginSettingsRepository
    {
        private PluginSettings current = new PluginSettings();

        public PluginSettings Load()
        {
            return current;
        }

        public void Save(PluginSettings settings)
        {
            current = settings;
        }
    }
}
