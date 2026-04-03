using CommEx.Infrastructure.Logging;
using CommEx.ViewModels.Settings;

namespace CommEx.Views.Settings
{
    /// <summary>
    /// UI 実装前に設定表示要求を受け取るためのプレースホルダです。
    /// </summary>
    internal class DeferredContextMenuSettingsView : IContextMenuSettingsView
    {
        private readonly IPluginLogger logger;

        public DeferredContextMenuSettingsView(IPluginLogger logger)
        {
            this.logger = logger;
        }

        public void Show(ContextMenuSettingsViewModel viewModel)
        {
            logger.Info($"Settings view requested. Sections={viewModel.Sections.Count}");
        }
    }
}
