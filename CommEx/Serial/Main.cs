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
using BveEx.Diagnostics;
using CommEx.Serial.ViewModel;
using CommEx.Serial.Views;
using CommEx.Serial.Common;
using CommEx.Serial.Bids;

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
        #region Plugin Settings

        /// <inheritdoc/>
        public override string Title { get; } = nameof(Serial);
        /// <inheritdoc/>
        public override string Description { get; } = "シリアル通信";

        #endregion

        #region Variables

        /// <summary>
        /// プラグインの有効・無効状態
        /// </summary>
        private bool status = true;

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
        private INative native;

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
        /// 設定ウィンドウ
        /// </summary>
        private readonly SettingWindow window;

        /// <summary>
        /// ビューモデル
        /// </summary>
        protected PortViewModel portViewModel;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public bool IsEnabled
        {
            get { return status; }
            set { status = value; }
        }

        #endregion

        #region Class Functions

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

            portViewModel = SaveSettings.Load();

            window = new SettingWindow(portViewModel);
            window.Closing += WindowClosing;
#if DEBUG
            window.Show();
#else
            window.Hide();
#endif
        }

        #endregion

        #region BveEx Functions

        /// <inheritdoc/>
        public override void Dispose()
        {
            SaveSettings.Save(portViewModel);

            Extensions.AllExtensionsLoaded -= AllExtensionsLoaded;
            BveHacker.ScenarioCreated -= OnScenarioCreated;
            BveHacker.ScenarioClosed -= ScenarioClosed;
            window.Closing -= WindowClosing;
            window.Close();
        }

        /// <inheritdoc/>
        public override void Tick(TimeSpan elapsed)
        {

        }

        #endregion

        #region BveEx Event Handlers

        /// <summary>
        /// 全ての BveEx 拡張機能が読み込まれ、BveEx.PluginHost.Plugins.Extensions プロパティが取得可能になると発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllExtensionsLoaded(object sender, EventArgs e)
        {
            bveHacker = BveHacker;
            cmx = Extensions.GetExtension<IContextMenuHacker>();
            native = Extensions.GetExtension<INative>();

            setting = cmx.AddCheckableMenuItem("シリアル通信設定", MenuItemCheckedChanged, ContextMenuItemType.CoreAndExtensions);
            native.Started += NativeStarted; ;

#if DEBUG
            setting.Checked = true;
#endif

            Bids.Bids.UpdateInfos(bveHacker, native);
        }

        /// <summary>
        /// シナリオ読み込み
        /// </summary>
        /// <param name="e"></param>
        private void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            scenario = e.Scenario;
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

        #endregion


        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void NativeStarted(object sender, StartedEventArgs e)
        {
#if DEBUG
            ErrorDialog.Show(new ErrorDialogInfo("started", "sender", "message"));
#else
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// 右クリックメニューのクリックイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 設定ウィンドウの閉じるボタンのクリックイベントハンドラ
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

        #endregion
    }
}
