using BveEx.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommEx.Serial
{
    /// <summary>
    /// シリアルループバック
    /// </summary>
    internal class Loopback : ISerialControl
    {
        /// <inheritdoc/>
        public void PortOpen(SerialPort serialPort)
        {
            serialPort.DataReceived += DataReceived;
        }

        /// <summary>
        /// シリアル受信時のイベントハンドラ 
        /// 送られてきた情報をそっくりそのまま返す
        /// </summary>
        /// <param name="sender">受信したポートの <see cref="SerialPort"/></param>
        /// <param name="e">イベントデータ</param>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort serialPort = (SerialPort)sender;
                string str = serialPort.ReadLine();
                serialPort.WriteLine(str);
            }
            catch (Exception ex)
            {
#if DEBUG
                ErrorDialog.Show(new ErrorDialogInfo("通信エラー", ex.Source, ex.Message));
#endif
            }
        }

        /// <inheritdoc/>
        public void PortClose(SerialPort serialPort)
        {
            serialPort.DataReceived -= DataReceived;
        }
    }
}
