using CommEx.Models.Settings;

namespace CommEx.Services.Settings
{
    /// <summary>
    /// 設定サービスの既定実装です。
    /// </summary>
    internal class PluginSettingsService : IPluginSettingsService
    {
        private readonly IPluginSettingsRepository repository;

        public PluginSettingsService(IPluginSettingsRepository repository)
        {
            this.repository = repository;
        }

        public PluginSettings Load()
        {
            return repository.Load();
        }

        public void Save(PluginSettings settings)
        {
            repository.Save(settings);
        }
    }
}
