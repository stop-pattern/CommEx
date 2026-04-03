using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommEx.Models.Settings;
using CommEx.Services.Settings;

namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// 右クリックメニューで表示する設定画面全体の ViewModel です。
    /// </summary>
    internal class ContextMenuSettingsViewModel
    {
        private readonly IPluginSettingsService settingsService;
        private readonly PluginSettings settings;

        public ContextMenuSettingsViewModel(
            IPluginSettingsService settingsService,
            IEnumerable<ISettingsSectionViewModelFactory> sectionFactories)
        {
            this.settingsService = settingsService;
            settings = settingsService.Load();

            List<ISettingsSectionViewModel> sections = new List<ISettingsSectionViewModel>();
            foreach (ISettingsSectionViewModelFactory factory in sectionFactories)
            {
                sections.Add(factory.Create(settings));
            }

            Sections = new ReadOnlyCollection<ISettingsSectionViewModel>(sections);
        }

        /// <summary>
        /// 設定セクション一覧を取得します。
        /// </summary>
        public IReadOnlyList<ISettingsSectionViewModel> Sections { get; }

        /// <summary>
        /// 現在の編集内容を保存します。
        /// </summary>
        public void Save()
        {
            settingsService.Save(settings);
        }
    }
}
