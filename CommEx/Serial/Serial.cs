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

namespace CommEx.Serial
{
    internal class SerialControl
    {
        private enum Errors
        {
            /// <summary>
            /// 原因不明エラー
            /// </summary>
            Unknown,
            /// <summary>
            /// コンバータとBIDSpp.dllとの間の接続が確立されていない
            /// </summary>
            NotConnected,
            /// <summary>
            /// 要求情報コードの数値部が不正
            /// </summary>
            ErrorInCodeNumber,
            /// <summary>
            /// 要求情報コードの記号部が不正
            /// </summary>
            ErrorInCodeSymbol,
            /// <summary>
            /// 識別子が不正
            /// </summary>
            ErrorInIdentifier,
            /// <summary>
            /// 数値変換がオーバーフローした
            /// </summary>
            Overflow,
            /// <summary>
            /// 要求情報コードの数値部に数値以外が混入している
            /// </summary>
            BadFormatInCode,
            /// <summary>
            /// 要求情報コードの数値部もしくは記号部が不正
            /// </summary>
            BadFormatCode,
            /// <summary>
            /// BVEのウィンドウハンドルを取得できない(キーイベント送信時)
            /// </summary>
            CantGetWindowHandle,
            /// <summary>
            /// (情報なし)
            /// </summary>
            NoInfo1,
            /// <summary>
            /// (情報なし)
            /// </summary>
            NoInfo2,
            /// <summary>
            /// (情報なし)
            /// </summary>
            NoInfo3,
            /// <summary>
            /// 配列の範囲外アクセス
            /// </summary>
            OutOfRange,
        }

        const int version = 300;

        private SerialPort port;

        private INative native;
        private IBveHacker bveHacker;

        /// <summary>
        /// 改行コード
        /// </summary>
        private string lineBreak = "\r\n";

        //IBveHacker bveHacker;
        //INative native;

        struct AutoSend
        {
            
        }


        internal void PortOpen()
        {
            port = new SerialPort();
            port.PortName = "COM0"; //SetPortName(_serialPort.PortName);
            port.BaudRate = 115200;//SetPortBaudRate(_serialPort.BaudRate);
            port.Parity = Parity.None;//SetPortParity(_serialPort.Parity);
            port.DataBits = 8;//SetPortDataBits(_serialPort.DataBits);
            port.StopBits = StopBits.One;//SetPortStopBits(_serialPort.StopBits);
            port.Handshake = Handshake.None;//SetPortHandshake(_serialPort.Handshake);
            port.Open();
        }
        internal void PortOpen(SerialPort serialPort)
        {
            port = serialPort;
            port.Open();
            port.DataReceived += DataReceived;
        }

        internal void Open()
        {
            port.NewLine = "\r\n";
            port.DataReceived += DataReceived;
            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "ポートオープンエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorDialogInfo info = new ErrorDialogInfo(
                    "ポートオープンエラー",
                    e.Source,
                    e.Message
                );
                ErrorDialog.Show(info);
                //throw;
            }
        }

        internal void PortClose()
        {
            port.Close();
            port.DataReceived -= DataReceived;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = port.ReadLine();
            str = str.Trim();
        }

        /// <summary>
        /// コマンドに応じた返答を生成
        /// </summary>
        /// <param name="str">コマンド</param>
        /// <returns>返答</returns>
        string CreateResponse(string str)
        {
            string header = str.Substring(0, 2).Trim();
            string body = str.Substring(2).Trim();
            string response = str.Trim() + "X";
            //int num = 0;

            if (header == "TR" || header == "EX")
            {
                switch (body.ElementAt(0))
                {
                    case 'A':   // 状態監視
                        return CreateError(Errors.ErrorInCodeSymbol);
                    case 'I':   // 運転情報
                        {
                            int num = 0;
                            if (!Convert.ToBoolean(int.TryParse(body.Substring(2), out num)))
                            {
                                CreateError(Errors.BadFormatInCode);
                            }

                            switch (body.ElementAt(1))
                            {
                                case 'C':   // Spec
                                    switch (num)
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
                                            CreateError(Errors.ErrorInCodeNumber);
                                            break;
                                    }
                                    break;
                                case 'E':   // Status
                                    switch (num)
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
                                        //    return response + bveHacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch.ToString();
                                        //    return response + bveHacker.Scenario.Vehicle.Instruments.Electricity.
                                        case 10: // 現在時刻(HH)[時]
                                            return response + native.VehicleState.Time.Hours.ToString();
                                        case 11: // 現在時刻(MM)[分]
                                            return response + native.VehicleState.Time.Minutes.ToString();
                                        case 12: // 現在時刻(SS)[秒]
                                            return response + native.VehicleState.Time.Seconds.ToString();
                                        case 13: // 現在時刻(ms)[ミリ秒]
                                            return response + native.VehicleState.Time.Milliseconds.ToString();
                                        default:
                                            CreateError(Errors.ErrorInCodeNumber);
                                            break;
                                    }
                                    break;
                                case 'H':   // Handle
                                    switch (num)
                                    {
                                        case 0: // Bノッチ位置
                                            return response + bveHacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch.ToString();
                                        case 1: // Pノッチ位置
                                            return response + bveHacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch.ToString();
                                        case 2: // レバーサー位置
                                            return response + bveHacker.Scenario.Vehicle.Instruments.Cab.Handles.ReverserPosition.ToString();
                                        case 3: // 定速状態（準備工事）
                                            return response + bveHacker.Scenario.Vehicle.Instruments.Cab.Handles.ConstantSpeedMode.ToString();
                                        default:
                                            return CreateError(Errors.ErrorInCodeNumber);
                                    }
                                case 'P':   // Panel
                                    for (int i = 0; i < body.Length; i++)
                                    {
                                        try
                                        {
                                            return response + native.AtsPanelArray[num].ToString();
                                            //int val = bveHacker.Scenario.Vehicle.Instruments.AtsPlugin.PanelArray[num];
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
                                            return response + native.AtsSoundArray[num].ToString();
                                            //int val = bveHacker.Scenario.Vehicle.Instruments.AtsPlugin.SoundArray[num];
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
                                    switch (num)
                                    {
                                        case 0:     // 全体
                                            return response + bveHacker.Scenario.Vehicle.Conductor.Doors.AreAllClosed;
                                        case -1:    // 左（準備工事）
                                        case 1:     // 右（準備工事）
                                        default:
                                            return CreateError(Errors.ErrorInCodeNumber);
                                    }
                                default:
                                    return CreateError(Errors.ErrorInCodeSymbol);
                            }
                            break;
                        }
                    case 'R':   // レバーサー操作要求
                        return CreateError(Errors.ErrorInCodeSymbol);
                    case 'S':   // ワンハンドル操作要求
                        return CreateError(Errors.ErrorInCodeSymbol);
                    case 'P':   // 力行操作要求
                        return CreateError(Errors.ErrorInCodeSymbol);
                    case 'B':   // 制動操作要求
                        return CreateError(Errors.ErrorInCodeSymbol);
                    case 'K':   // キー操作要求
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
            }
            return null;
        }

        string CreateError(Errors err, string header = "EX")
        {
#if DEBUG
            Debug.WriteLine(err.ToString());
#endif
            return header + "E" + err.ToString();
        }

    }
}
