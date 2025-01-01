using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using BveEx.Extensions.ContextMenuHacker;
using BveEx.PluginHost;
//using BveEx.PluginHost.Scripting;
using BveEx.PluginHost.Plugins;
using BveEx.PluginHost.Plugins.Extensions;
using BveTypes.ClassWrappers;
using BveEx.Extensions.Native;

namespace CommEx.Serial
{
    /// <summary>
    /// プラグインの本体
    /// Plugin() の第一引数でこのプラグインの仕様を指定
    /// Plugin() の第二引数でこのプラグインが必要とするBveEx本体の最低バージョンを指定（オプション）
    /// </summary>
    [Plugin(PluginType.Extension)]
    [Togglable]
    internal class Serial : AssemblyPluginBase, ITogglableExtension, IExtension
    {
        /// <inheritdoc/>
        public override string Title { get; } = nameof(Serial);
        /// <inheritdoc/>
        public override string Description { get; } = "シリアル通信";

        /// <summary>
        /// シナリオ
        /// </summary>
        private Scenario scenario;

        /// <summary>
        /// BveHacker
        /// </summary>
        private IBveHacker bveHacker;

        /// <summary>
        /// Native
        /// </summary>
        INative native;

        /// <summary>
        /// 右クリックメニュー操作用
        /// ContextMenuHacker
        /// </summary>
        private IContextMenuHacker cmx;

        /// <summary>
        /// 右クリックメニューの設定ボタン
        /// </summary>
        private ToolStripMenuItem setting;

        /// <summary>
        /// プラグインの有効・無効状態
        /// </summary>
        private bool status = true;

        /// <inheritdoc/>
        public bool IsEnabled
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 設定ウィンドウ
        /// </summary>
        private readonly SettingWindow window;

        /// <summary>
        /// プラグインが読み込まれた時に呼ばれる
        /// 初期化を実装する
        /// </summary>
        /// <param name="builder"></param>
        public Serial(PluginBuilder builder) : base(builder)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.AutoFlush = true;
            Extensions.AllExtensionsLoaded += AllExtensionsLoaded;
            BveHacker.ScenarioCreated += OnScenarioCreated;
            BveHacker.ScenarioClosed += ScenarioClosed;
            window = new SettingWindow();
            window.Closing += WindowClosing;
            window.Hide();
        }

        /// <summary>
        /// 閉じるボタンのコールバック
        /// </summary>
        /// <param name="sender"><see cref="SettingWindow">SettingWindow</see></param>
        /// <param name="e">キャンセルできるイベントのデータ</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            SettingWindow window = (SettingWindow)sender;
            window.Hide();
            setting.Checked = false;
        }

        /// <summary>
        /// 全ての BveEx 拡張機能が読み込まれ、BveEx.PluginHost.Plugins.Extensions プロパティが取得可能になると発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllExtensionsLoaded(object sender, EventArgs e)
        {
            cmx = Extensions.GetExtension<IContextMenuHacker>();
            setting = cmx.AddCheckableMenuItem("シリアル通信設定", MenuItemCheckedChanged, ContextMenuItemType.CoreAndExtensions);
        }

        /// <summary>
        /// 右クリックメニューのボタンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MenuItemCheckedChanged(object sender, EventArgs e)
        {
            if (setting != null)
            {
            if (setting.Checked)
            {
                window.Show();
            }
            else
            {
                window.Hide();
            }
        }
        }

        /// <summary>
        /// プラグインが解放されたときに呼ばれる
        /// 後処理を実装する
        /// </summary>
        public override void Dispose()
        {
            Extensions.AllExtensionsLoaded -= AllExtensionsLoaded;
            BveHacker.ScenarioCreated -= OnScenarioCreated;
            BveHacker.ScenarioClosed -= ScenarioClosed;
            window.Closing -= WindowClosing;
            window.Close();
        }

        /// <summary>
        /// シナリオ読み込み中に毎フレーム呼び出される
        /// </summary>
        /// <param name="elapsed">前回フレームからの経過時間</param>
        public override void Tick(TimeSpan elapsed)
        {

        }

        /// <summary>
        /// シナリオ読み込み
        /// </summary>
        /// <param name="e"></param>
        private void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            scenario = e.Scenario;
            bveHacker = BveHacker;

            //Bids.Load(bveHacker);

            //try
            //{
            //    _ExtendedBeacons = ExtendedBeaconSet.Load(Native, BveHacker, e.Scenario);
            //}
            //catch (Exception ex)
            //{
            //    switch (ex)
            //    {
            //        case BveFileLoadException exception:
            //            BveHacker.LoadErrorManager.Throw(exception.Message, exception.SenderFileName, exception.LineIndex, exception.CharIndex);
            //            break;

            //        case CompilationException exception:
            //            foreach (Diagnostic diagnostic in exception.CompilationErrors)
            //            {
            //                string message = diagnostic.GetMessage();
            //                string fileName = Path.GetFileName(diagnostic.Location.SourceTree.FilePath);

            //                LinePosition position = diagnostic.Location.GetLineSpan().StartLinePosition;
            //                int lineIndex = position.Line;
            //                int charIndex = position.Character;

            //                BveHacker.LoadErrorManager.Throw(message, fileName, lineIndex, charIndex);
            //            }
            //            break;

            //        default:
            //            BveHacker.LoadErrorManager.Throw(ex.Message);
            //            _ = MessageBox.Show(ex.ToString(), App.Instance.ProductName);
            //            break;
            //    }
            //}
        }

        /// <summary>
        /// シナリオ終了
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ScenarioClosed(EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
