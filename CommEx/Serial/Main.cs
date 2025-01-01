﻿using System;
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
        private SettingWindow window = new SettingWindow();

        /// <summary>
        /// プラグインが読み込まれた時に呼ばれる
        /// 初期化を実装する
        /// </summary>
        /// <param name="builder"></param>
        public Serial(PluginBuilder builder) : base(builder)
        {
            Extensions.AllExtensionsLoaded += Extensions_AllExtensionsLoaded;
        }

        /// <summary>
        /// 全ての AtsEX 拡張機能が読み込まれ、AtsEx.PluginHost.Plugins.Extensions プロパティが取得可能になると発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Extensions_AllExtensionsLoaded(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// プラグインが解放されたときに呼ばれる
        /// 後処理を実装する
        /// </summary>
        public override void Dispose()
        {
            Extensions.AllExtensionsLoaded -= Extensions_AllExtensionsLoaded;
        }

        /// <summary>
        /// シナリオ読み込み中に毎フレーム呼び出される
        /// </summary>
        /// <param name="elapsed">前回フレームからの経過時間</param>
        public override TickResult Tick(TimeSpan elapsed)
        {
            return new ExtensionTickResult();
        }
    }
}
