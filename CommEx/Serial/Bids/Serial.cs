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

        List<BidsData> bids = null;
        List<BidsData> autosend = null;

        /// <summary>
        /// 改行コード
        /// </summary>
        private string lineBreak = "\r\n";

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
            serialPort.NewLine = lineBreak;
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
            string str = "";
            try
            {
                str = port.ReadLine();
            }
            catch (Exception ex)
            {
#if DEBUG
                ErrorDialog.Show(new ErrorDialogInfo("エラー：シリアル読み込み失敗", ex.Source, ex.Message));
#endif
                return;
            }
            str = str.Trim();
            Debug.Print("Serial Receive Data" + str);

            if (str.Length < 5)
            {
                return;
            }

            if (str.StartsWith("EX") || str.StartsWith("TR"))
            {
                string response;
                if (native == null)
                {
                    response = CreateError(Errors.NotStarted);
                }
                else if (!isAvailable)
                {
                    response = CreateError(Errors.NotStarted);
                }
                else
                {
                    response = CreateResponse(str);
                }

                if (response != null)
                {
                    Debug.Print("Serial Send Data" + response);
                    try
                    {
                        port.WriteLine(response);
                    }
                    catch (Exception ex)
                    {
                        ErrorDialog.Show(new ErrorDialogInfo("ポートが無効状態です。", ex.Source, ex.Message));
                    }
                }
            }
        }

        #endregion
    }
}
