using CommEx.Models.Settings;

namespace CommEx.Services.Settings
{
    /// <summary>
    /// プラグイン設定の永続化境界を表します。
    /// </summary>
    internal interface IPluginSettingsRepository
    {
        PluginSettings Load();

        void Save(PluginSettings settings);
    }
}
