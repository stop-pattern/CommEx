namespace CommEx.ViewModels.Settings
{
    /// <summary>
    /// 右クリックメニューの設定セクションを表す ViewModel 契約です。
    /// </summary>
    internal interface ISettingsSectionViewModel
    {
        string SectionKey { get; }

        string DisplayName { get; }

        bool IsEnabled { get; set; }
    }
}
