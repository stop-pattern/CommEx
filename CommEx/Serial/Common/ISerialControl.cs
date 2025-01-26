using BveEx.PluginHost;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommEx.Serial.Common
{
    public interface ISerialControl
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

    /// <summary>
    /// 入力されたキーを送信する
    /// </summary>
    /// <param name="nInputs"></param>
    /// <param name="pInputs"></param>
    /// <param name="cbsize"></param>
    //[DllImport("user32.dll", SetLastError = true)]
    //public extern static void SendInput(int nInputs, Input[] pInputs, int cbsize);

    /// <summary>
    /// BveHacker と Native の初期化
    /// </summary>
    /// <param name="bveHacker"></param>
    /// <param name="native"></param>
    /// <exception cref="ArgumentNullException">引数がnullの時に投げる例外</exception>
    //public static void Load(IBveHacker bveHacker, INative native)
    //{
    //    native = native ?? throw new ArgumentNullException(nameof(native));
    //    bveHacker = bveHacker ?? throw new ArgumentNullException(nameof(bveHacker));
    //}
    //public static void Load(IBveHacker bveHacker)
    //{
    //    bveHacker = bveHacker ?? throw new ArgumentNullException(nameof(bveHacker));
    //}

}
