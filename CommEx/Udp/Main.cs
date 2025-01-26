using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveEx.PluginHost.Plugins;
using BveEx.PluginHost.Plugins.Extensions;

namespace CommEx.Udp
{
    /// <summary>
    /// プラグインの本体
    /// Plugin() の第一引数でこのプラグインの仕様を指定
    /// Plugin() の第二引数でこのプラグインが必要とするBveEx本体の最低バージョンを指定（オプション）
    /// </summary>
    [Plugin(PluginType.Extension)]
    [Togglable]
    internal class Udp : AssemblyPluginBase, ITogglableExtension, IExtension
    {
        #region Plugin Settings

        /// <inheritdoc/>
        public override string Title { get; } = nameof(Udp);
        /// <inheritdoc/>
        public override string Description { get; } = "UDP";

        #endregion

        #region Variables

        /// <summary>
        /// プラグインの有効・無効状態
        /// </summary>
        private bool status = false;

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
        public Udp(PluginBuilder builder) : base(builder)
        {
        }

        #endregion

        #region BveEx Functions

        /// <inheritdoc/>
        public override void Tick(TimeSpan elapsed)
        {
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }

        #endregion
    }
}
