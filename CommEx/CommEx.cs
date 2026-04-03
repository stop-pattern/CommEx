using System;

using BveEx.PluginHost.Plugins;
using BveEx.PluginHost.Plugins.Extensions;

using CommEx.App;
using CommEx.ViewModels;

namespace CommEx
{
    /// <summary>
    /// プラグインの本体
    /// Plugin() の第一引数でこのプラグインの仕様を指定
    /// Plugin() の第二引数でこのプラグインが必要とするBveEx本体の最低バージョンを指定（オプション）
    /// Togglable を付加するとユーザーがBveExのバージョン一覧から有効・無効を切換できる
    /// </summary>
    [Plugin(PluginType.Extension)]
    [Togglable]
    internal class CommExMain : AssemblyPluginBase, ITogglableExtension, IExtension
    {
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// プラグインが読み込まれた時に呼ばれる
        /// 初期化を実装する
        /// </summary>
        public CommExMain(PluginBuilder builder) : base(builder)
        {
            mainViewModel = PluginCompositionRoot.BuildMainViewModel();
        }

        /// <summary>
        /// プラグインの有効・無効状態
        /// </summary>
        public bool IsEnabled
        {
            get { return mainViewModel.IsEnabled; }
            set { mainViewModel.IsEnabled = value; }
        }

        /// <summary>
        /// プラグインが解放されたときに呼ばれる
        /// 後処理を実装する
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// シナリオ読み込み中に毎フレーム呼び出される
        /// </summary>
        /// <param name="elapsed">前回フレームからの経過時間</param>
        public override void Tick(TimeSpan elapsed)
        {
            mainViewModel.OnTick(elapsed);
        }
    }
}
