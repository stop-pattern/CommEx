using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// 設定モデルからセクション ViewModel を生成するファクトリ契約です。
    /// </summary>
    internal interface ISettingsSectionViewModelFactory
    {
        ISettingsSectionViewModel Create(PluginSettings settings);
    }
}
