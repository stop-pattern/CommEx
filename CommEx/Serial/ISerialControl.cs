using BveEx.PluginHost;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommEx.Serial
{
    internal interface ISerialControl
    {
        /// <summary>
        /// ポートを開ける前に呼ばれる
        /// </summary>
        /// <param name="serialPort"><see cref="SerialPort"/></param>
        void PortOpen(SerialPort serialPort);

        /// <summary>
        /// ポートを閉じた後に呼ばれる
        /// </summary>
        /// <param name="serialPort"><see cref="SerialPort"/></param>
        void PortClose(SerialPort serialPort);

        /// <summary>
        /// シリアルポートの受信時に呼ばれる
        /// </summary>
        /// <param name="sender"><see cref="SerialPort"/></param>
        /// <param name="e"></param>
        //void DataReceived(object sender, SerialDataReceivedEventArgs e);
    }

    interface IBveEx
    {
        /// <summary>
        /// 全ての BveEx 拡張機能が読み込まれ、BveEx.PluginHost.Plugins.Extensions プロパティが取得可能になると発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AllExtensionsLoaded(object sender, EventArgs e);

        /// <summary>
        /// シナリオ読み込み
        /// </summary>
        /// <param name="e"></param>
        void OnScenarioCreated(ScenarioCreatedEventArgs e);

        /// <summary>
        /// シナリオ読み込み中に毎フレーム呼び出される
        /// </summary>
        /// <param name="elapsed"></param>
        void Tick(TimeSpan elapsed);

        /// <summary>
        /// シナリオ終了
        /// </summary>
        /// <param name="e"></param>
        void ScenarioClosed(EventArgs e);
    }
}
