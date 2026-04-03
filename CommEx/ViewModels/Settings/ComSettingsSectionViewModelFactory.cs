using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// COM 設定セクションの ViewModel を生成します。
    /// </summary>
    internal class ComSettingsSectionViewModelFactory : ISettingsSectionViewModelFactory
    {
        public ISettingsSectionViewModel Create(PluginSettings settings)
        {
            return new ComSettingsSectionViewModel(settings.ComPorts);
        }
    }
}
