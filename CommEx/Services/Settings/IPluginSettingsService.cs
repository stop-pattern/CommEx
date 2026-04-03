using CommEx.Models.Settings;

namespace CommEx.Services.Settings
{
    /// <summary>
    /// 設定の読込/保存ユースケースを提供します。
    /// </summary>
    internal interface IPluginSettingsService
    {
        PluginSettings Load();

        void Save(PluginSettings settings);
    }
}
