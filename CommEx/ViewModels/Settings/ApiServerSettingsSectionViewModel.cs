using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// API サーバー設定セクションの ViewModel です。
    /// </summary>
    internal class ApiServerSettingsSectionViewModel : ISettingsSectionViewModel
    {
        private readonly ApiServerSettings settings;

        public ApiServerSettingsSectionViewModel(ApiServerSettings settings)
        {
            this.settings = settings;
        }

        public string SectionKey => "api";

        public string DisplayName => "API";

        public bool IsEnabled
        {
            get { return settings.IsEnabled; }
            set { settings.IsEnabled = value; }
        }

        public int Port
        {
            get { return settings.Port; }
            set { settings.Port = value; }
        }
    }
}
