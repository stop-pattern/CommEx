using System.Collections.Generic;
using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// UDP 設定セクションの ViewModel です。
    /// </summary>
    internal class UdpSettingsSectionViewModel : ISettingsSectionViewModel
    {
        private readonly UdpSettings settings;

        public UdpSettingsSectionViewModel(UdpSettings settings)
        {
            this.settings = settings;
        }

        public string SectionKey => "udp";

        public string DisplayName => "UDP";

        public bool IsEnabled
        {
            get { return settings.IsEnabled; }
            set { settings.IsEnabled = value; }
        }

        public IList<UdpNicSettings> NicSettings => settings.Nics;
    }
}
