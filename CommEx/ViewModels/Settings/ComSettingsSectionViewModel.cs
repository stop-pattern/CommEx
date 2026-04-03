using System.Collections.Generic;
using CommEx.Models.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// COM 設定セクションの ViewModel です。
    /// </summary>
    internal class ComSettingsSectionViewModel : ISettingsSectionViewModel
    {
        private readonly IList<ComPortSettings> comPorts;

        public ComSettingsSectionViewModel(IList<ComPortSettings> comPorts)
        {
            this.comPorts = comPorts;
        }

        public string SectionKey => "com";

        public string DisplayName => "COM";

        public bool IsEnabled
        {
            get
            {
                for (int i = 0; i < comPorts.Count; i++)
                {
                    if (comPorts[i].IsEnabled)
                    {
                        return true;
                    }
                }

                return false;
            }
            set
            {
                for (int i = 0; i < comPorts.Count; i++)
                {
                    comPorts[i].IsEnabled = value;
                }
            }
        }

        public IList<ComPortSettings> Ports => comPorts;

        public ComPortSettings AddPort()
        {
            ComPortSettings newPort = new ComPortSettings
            {
                PortName = $"COM{comPorts.Count + 1}"
            };

            comPorts.Add(newPort);
            return newPort;
        }
    }
}
