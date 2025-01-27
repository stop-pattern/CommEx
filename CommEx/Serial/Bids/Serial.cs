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

        private const int version = 300;

        private static bool isAvailable = false;

        private static IBveHacker hacker;
        private static INative native;

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
        /// インスタンスの取り込み
        /// </summary>
        public static void UpdateInfos(IBveHacker bveHacker, INative bveNative)
        {
            hacker = bveHacker;
            native = bveNative;
        }

        /// <summary>
        /// 使用可否を設定
        /// </summary>
        /// <param name="status">使用可否</param>
        public static void SetStatus(bool status)
        {
            isAvailable = status;
        }

        /// <summary>
        /// コマンドに応じた返答を生成
        /// </summary>
        /// <param name="str">コマンド</param>
        /// <returns>返答</returns>
        private string CreateResponse(string str)
        {
            string header = str.Substring(0, 2).Trim();
            string body = str.Substring(2).Trim();
            string response = str.Trim() + "X";

            int num1 = 0;
            if (!Convert.ToBoolean(int.TryParse(body.Substring(1), out num1)))
            {
                if (body.ElementAt(0) != 'I')
                {
                    return CreateError(Errors.BadFormatInCode);
                }
            }

            switch (body.ElementAt(0))
            {
                case 'A':   // 状態監視
                    return CreateError(Errors.ErrorInCodeSymbol);
                case 'I':   // 運転情報
                    int num2 = 0;
                    if (!Convert.ToBoolean(int.TryParse(body.Substring(2), out num2)))
                    {
                        CreateError(Errors.BadFormatInCode);
                    }

                    switch (body.ElementAt(1))
                    {
                        case 'C':   // Spec
                            switch (num2)
                            {
                                case 0: // Bノッチ数
                                    return response + native.VehicleSpec.BrakeNotches.ToString();
                                case 1: // Pノッチ数
                                    return response + native.VehicleSpec.PowerNotches.ToString();
                                case 2: // ATS確認段
                                    return response + native.VehicleSpec.AtsNotch.ToString();
                                case 3: // B67相当段
                                    return response + native.VehicleSpec.B67Notch.ToString();
                                case 4: // 車両編成数
                                    return response + native.VehicleSpec.Cars.ToString();
                                default:
                                    return CreateError(Errors.ErrorInCodeNumber);
                            }
                        case 'E':   // Status
                            switch (num2)
                            {
                                case 0: // 列車位置[m]
                                    return response + native.VehicleState.Location.ToString();
                                case 1: // 列車速度[km/h]
                                    return response + native.VehicleState.Speed.ToString();
                                case 2: // 現在時刻[ms]
                                    return response + native.VehicleState.Time.TotalMilliseconds.ToString();
                                case 3: // BC Pres[kPa]
                                    return response + native.VehicleState.BcPressure.ToString();
                                case 4: // MR Pres [kPa]
                                    return response + native.VehicleState.MrPressure.ToString();
                                case 5: // ER Pres [kPa]
                                    return response + native.VehicleState.ErPressure.ToString();
                                case 6: // BP Pres [kPa]
                                    return response + native.VehicleState.BpPressure.ToString();
                                case 7: // SAP Pres [kPa]
                                    return response + native.VehicleState.SapPressure.ToString();
                                case 8: // 電流 [A]
                                    return response + native.VehicleState.Current.ToString();
                                //case 9: // 電圧 [V]（準備工事）
                                //    return response + hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch.ToString();
                                //    return response + hacker.Scenario.Vehicle.Instruments.Electricity.
                                case 10: // 現在時刻(HH)[時]
                                    return response + native.VehicleState.Time.Hours.ToString();
                                case 11: // 現在時刻(MM)[分]
                                    return response + native.VehicleState.Time.Minutes.ToString();
                                case 12: // 現在時刻(SS)[秒]
                                    return response + native.VehicleState.Time.Seconds.ToString();
                                case 13: // 現在時刻(ms)[ミリ秒]
                                    return response + native.VehicleState.Time.Milliseconds.ToString();
                                default:
                                    return CreateError(Errors.ErrorInCodeNumber);
                            }
                        case 'H':   // Handle
                            switch (num2)
                            {
                                case 0: // Bノッチ位置
                                    return response + hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch.ToString();
                                case 1: // Pノッチ位置
                                    return response + hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch.ToString();
                                case 2: // レバーサー位置
                                    return response + hacker.Scenario.Vehicle.Instruments.Cab.Handles.ReverserPosition.ToString();
                                case 3: // 定速状態（準備工事）
                                    return response + hacker.Scenario.Vehicle.Instruments.Cab.Handles.ConstantSpeedMode.ToString();
                                default:
                                    return CreateError(Errors.ErrorInCodeNumber);
                            }
                        case 'P':   // Panel
                            for (int i = 0; i < body.Length; i++)
                            {
                                try
                                {
                                    return response + native.AtsPanelArray[num2].ToString();
                                    //int val = hacker.Scenario.Vehicle.Instruments.AtsPlugin.PanelArray[num2];
                                    //return response + val.ToString();
                                }
                                catch (Exception e)
                                {
#if DEBUG
                                    ErrorDialogInfo errorDialogInfo = new ErrorDialogInfo("エラー：配列の範囲外アクセス", e.Source, e.Message);
                                    ErrorDialog.Show(errorDialogInfo);
#endif
                                    return CreateError(Errors.OutOfRange);
                                }
                            }
                            return CreateError(Errors.ErrorInCodeNumber);
                        case 'S':   // Sound
                            for (int i = 0; i < body.Length; i++)
                            {
                                try
                                {
                                    return response + native.AtsSoundArray[num2].ToString();
                                    //int val = hacker.Scenario.Vehicle.Instruments.AtsPlugin.SoundArray[num2];
                                    //return response + val.ToString();
                                }
                                catch (Exception e)
                                {
#if DEBUG
                                    ErrorDialogInfo errorDialogInfo = new ErrorDialogInfo("エラー：配列の範囲外アクセス", e.Source, e.Message);
                                    ErrorDialog.Show(errorDialogInfo);
#endif
                                    return CreateError(Errors.OutOfRange);
                                }
                            }
                            return CreateError(Errors.ErrorInCodeNumber);
                        case 'D':   // ドア状態
                            switch (num2)
                            {
                                case 0:     // 全体
                                    return response + hacker.Scenario.Vehicle.Conductor.Doors.AreAllClosed;
                                case -1:    // 左（準備工事）
                                case 1:     // 右（準備工事）
                                default:
                                    return CreateError(Errors.ErrorInCodeNumber);
                            }
                        default:
                            return CreateError(Errors.ErrorInCodeSymbol);
                    }
                case 'R':   // レバーサー操作要求
                    if (-1 <= num1 && num1 <= 1)
                    {
                        hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch = num1;
                        return response + 0.ToString();
                    }
                    return CreateError(Errors.ErrorInCodeSymbol);
                case 'S':   // ワンハンドル操作要求
                    //if (num1 > 0)
                    //{
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch += num1;
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch += num1;
                    //    return response + 0.ToString();
                    //}
                    //else if (num1 < 0)
                    //{
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch += num1;
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch += num1;
                    //    return response + 0.ToString();
                    //}
                    //else if (num1 == 0)
                    //{
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch = 0;
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch = 0;
                    //    return response + 0.ToString();
                    //}
                    return CreateError(Errors.ErrorInCodeSymbol);
                case 'P':   // 力行操作要求
                    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch += num1;
                    return response + 0.ToString();
                case 'B':   // 制動操作要求
                    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch += num1;
                    return response + 0.ToString();
                case 'K':   // キー操作要求
                    switch (body.ElementAt(1))
                    {
                        case 'P':   // Pless
                            if (num1 <= (int)AtsKeyName.L)
                            {
                                //hacker.InputManager.KeyDown_Invoke(InputEventArgsFactory.AtsKey((AtsKeyName)num1));
                                return response + 0.ToString();
                            }
                            return CreateError(Errors.ErrorInCodeNumber);
                        case 'R':   // Release
                            if (num1 <= (int)AtsKeyName.L)
                            {
                                //hacker.InputManager.KeyUp_Invoke(InputEventArgsFactory.AtsKey((AtsKeyName)num1));
                                return response + 0.ToString();
                            }
                            return CreateError(Errors.ErrorInCodeNumber);
                        default:
                            return CreateError(Errors.ErrorInCodeSymbol);
                    }
                    return CreateError(Errors.ErrorInCodeSymbol);
                case 'V':   // バージョン情報
                    return header + version.ToString();
                case 'E':   // エラー情報
                    return CreateError(Errors.ErrorInCodeSymbol);
                case 'H':   // 保安装置情報
                    return CreateError(Errors.ErrorInCodeSymbol);
                default:
                    return CreateError(Errors.ErrorInCodeSymbol);
            }
            return null;
        }

        private string CreateError(Errors err, string header = "EX")
        {
#if DEBUG
            Debug.WriteLine(err.ToString());
#endif
            return header + "E" + (int)err;
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
