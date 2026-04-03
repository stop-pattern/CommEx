using CommEx.ViewModels.Settings;

namespace CommEx.Views.Settings
{
    /// <summary>
    /// Bve の右クリックメニューから呼び出される設定表示のアダプタ契約です。
    /// </summary>
    internal interface IContextMenuSettingsView
    {
        void Show(ContextMenuSettingsViewModel viewModel);
    }
}
