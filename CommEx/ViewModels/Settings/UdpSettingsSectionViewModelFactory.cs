using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// UDP 設定セクションの ViewModel を生成します。
    /// </summary>
    internal class UdpSettingsSectionViewModelFactory : ISettingsSectionViewModelFactory
    {
        public ISettingsSectionViewModel Create(PluginSettings settings)
        {
            return new UdpSettingsSectionViewModel(settings.Udp);
        }
    }
}
