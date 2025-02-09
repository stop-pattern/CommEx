using BveEx.PluginHost;
using BveEx.Diagnostics;
using BveEx.Extensions.Native;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Windows.Input;
using BveEx.PluginHost.Input;
using BveEx.Extensions.Native.Input;
using System.Windows.Media.Animation;
using CommEx.Serial.Common;

namespace CommEx.Serial.Bids
{
    public class BidsSerial : ISerialControl
    {
        #region Fields

        List<BidsData> bids = new List<BidsData>();
        List<BidsData> autosend = new List<BidsData>();
        Queue<BidsData> received = new Queue<BidsData>();
        Queue<BidsData> sending = new Queue<BidsData>();

        #endregion

        #region Structs

        struct AutoSend
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// シナリオ開始通知
        /// インスタンスの取り込み
        /// </summary>
        public static void ScenarioStart(IBveHacker bveHacker, INative bveNative)
        {
            Bve.UpdateStatus(true, bveNative, bveHacker);
        }

        /// <summary>
        /// シナリオ終了通知
        /// </summary>
        public static void ScenarioEnd()
        {
            Bve.UpdateStatus();
        }

        #endregion

        #region Interface Implementation

        /// <inheritdoc/>
        public void PortOpen(SerialPort serialPort)
        {
            serialPort.DataReceived += DataReceived;
        }

        /// <inheritdoc/>
        public void PortClose(SerialPort serialPort)
        {
            serialPort.DataReceived -= DataReceived;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// シリアルポートの受信時に呼ばれる
        /// </summary>
        /// <param name="sender"><see cref="SerialPort"/></param>
        /// <param name="e">event args</param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;

            // シリアルポートが有効か確認
            try
            {
                if (!port.IsOpen)
            {
                    Debug.WriteLine("Serial Already Closed");
                    //ErrorDialog.Show(new ErrorDialogInfo("エラー：シリアルポートが閉じられています", null, null));
                return;
            }

                if (!port.BaseStream.CanRead || !port.BaseStream.CanWrite)
            {
                    Debug.WriteLine($"Read or Write not allowed: CanRead:{port.BaseStream.CanRead}, CanWrite:{port.BaseStream.CanWrite}");
                return;
            }

                if (port.BytesToRead == 0)
            {
                    Debug.WriteLine("No Data");
                    return;
                }
            }
            catch (Exception ex)
                {
                ErrorDialog.Show(new ErrorDialogInfo("エラー：シリアル読み込み失敗", ex.Source, ex.Message));
                }

            // 受信値を取得
            string str = "";
            try
                {
                str = port.ReadExisting();
                }
            catch (Exception ex)
                {
#if DEBUG
                ErrorDialog.Show(new ErrorDialogInfo("エラー：シリアル読み込み失敗", ex.Source, ex.Message));
#endif
                return;
                }


            // 改行文字ごとに分割
            string[] lines = str.Split(new string[] { port.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Print("Serial Receive Data: " + lines);

            // 受信データをパース
            foreach (var line in lines)
                {
                line.Trim();
                if (line.Length < 5) continue;

                if (BidsData.TryParse(line, out BidsData data))
                    {
                    Debug.WriteLine("Parse Error");
                    continue;
                    }
                else
                    {
                    if (data.Header != "TR" && data.Header != "EX") continue;
                    received.Enqueue(data);
                }
            }
        }

        #endregion
    }
}
