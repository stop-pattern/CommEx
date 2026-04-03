using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// API サーバー設定セクションの ViewModel を生成します。
    /// </summary>
    internal class ApiServerSettingsSectionViewModelFactory : ISettingsSectionViewModelFactory
    {
        public ISettingsSectionViewModel Create(PluginSettings settings)
        {
            return new ApiServerSettingsSectionViewModel(settings.ApiServer);
        }
    }
}
