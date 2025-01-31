using BveEx.Diagnostics;
using BveEx.Extensions.Native;
using BveEx.PluginHost;
using BveEx.PluginHost.Input;
using BveTypes.ClassWrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace CommEx.Serial.Bids
{
    internal static class Bve
    {
        #region Fields

        private const int version = 300;

        private static bool isAvailable = false;

        private static IBveHacker hacker = null;
        private static INative native = null;

        #endregion

        #region Static Methods

        /// <summary>
        /// 利用可否を設定
        /// </summary>
        /// <param name="status">シナリオ情報利用可否</param>
        public static void UpdateStatus(bool status = false)
        {
            isAvailable = status;
        }

        /// <summary>
        /// 利用可否を設定
        /// </summary>
        /// <param name="status">シナリオ情報利用可否</param>
        /// <param name="bveNative"><see cref="INative"/> インスタンス</param>
        public static void UpdateStatus(bool status, INative bveNative)
        {
            native = bveNative;
            UpdateStatus(status);
        }

        /// <summary>
        /// 利用可否を設定
        /// </summary>
        /// <param name="status">シナリオ情報利用可否</param>
        /// <param name="bveNative"><see cref="INative"/> インスタンス</param>
        /// <param name="bveHacker"><see cref="IBveHacker"/> インスタンス</param>
        public static void UpdateStatus(bool status, INative bveNative, IBveHacker bveHacker)
        {
            hacker = bveHacker;
            UpdateStatus(status, bveNative);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 返却値を設定
        /// </summary>
        /// <returns>設定後のインスタンス</returns>
        public static BidsData CheckIdentifier(BidsData data)
        {
            switch (data.Identifier)
            {
                case 'A':   // 状態監視
                    return data.SetError(Errors.ErrorInCodeSymbol);
                case 'I':   // 運転情報
                    return CheckInfo(data);
                case 'R':   // レバーサー操作要求
                    var rev = hacker.Scenario.Vehicle.Instruments.Cab.Handles.ReverserPosition;
                    switch (data.Info)
                    {
                        case 'F':   // Forward
                            rev = ReverserPosition.F;
                            return data.SetValue((int)rev);
                        case 'N':   // Neutral
                            rev = ReverserPosition.N;
                            return data.SetValue((int)rev);
                        case 'R':   // Reverse
                            rev = ReverserPosition.B;
                            return data.SetValue((int)rev);
                        default:
                            switch (data.Code)
                            {
                                case 1:   // Forward
                                    rev = ReverserPosition.F;
                                    return data.SetValue((int)rev);
                                case 0:   // Neutral
                                    rev = ReverserPosition.N;
                                    return data.SetValue((int)rev);
                                case -1:   // Reverse
                                    rev = ReverserPosition.B;
                                    return data.SetValue((int)rev);
                                default:
                                    //return SetError(Errors.ErrorInCodeNumber);
                                    return data.SetError(Errors.ErrorInCodeSymbol);
                            }
                    }
                case 'S':   // ワンハンドル操作要求
                    //if (num1 > 0)
                    //{
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch += num1;
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch += num1;
                    //    return SetValue(0);
                    //}
                    //else if (num1 < 0)
                    //{
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch += num1;
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch += num1;
                    //    return SetValue(0);
                    //}
                    //else if (num1 == 0)
                    //{
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch = 0;
                    //    hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch = 0;
                    //    return SetValue(0);
                    //}
                    return data.SetError(Errors.ErrorInCodeSymbol);
                case 'P':   // 力行操作要求
                    var power = hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch;
                    power += data.Code;
                    return data.SetValue(power);
                case 'B':   // 制動操作要求
                    var brake = hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch;
                    brake += data.Code;
                    return data.SetValue(brake);
                case 'K':   // キー操作要求
                    switch (data.Info)
                    {
                        case 'P':   // Pless
                            if (data.Code <= (int)AtsKeyName.L)
                            {
                                //hacker.InputManager.KeyDown_Invoke(InputEventArgsFactory.AtsKey((AtsKeyName)num1));
                                return data.SetValue(0);
                            }
                            return data.SetError(Errors.ErrorInCodeNumber);
                        case 'R':   // Release
                            if (data.Code <= (int)AtsKeyName.L)
                            {
                                //hacker.InputManager.KeyUp_Invoke(InputEventArgsFactory.AtsKey((AtsKeyName)num1));
                                return data.SetValue(0);
                            }
                            return data.SetError(Errors.ErrorInCodeNumber);
                        default:
                            return data.SetError(Errors.ErrorInCodeSymbol);
                    }
                case 'V':   // バージョン情報
                    return data.SetValue(version);
                case 'E':   // エラー情報
                    return data.SetError(Errors.ErrorInCodeSymbol);
                case 'H':   // 保安装置情報
                    return data.SetError(Errors.ErrorInCodeSymbol);
                default:
                    return data.SetError(Errors.ErrorInCodeSymbol);
            }
        }

        /// <summary>
        /// 運転情報要求の返却値を設定
        /// </summary>
        /// <returns>設定後のインスタンス</returns>
        private static BidsData CheckInfo(BidsData data)
        {
            switch (data.Info)
            {
                case 'C':   // Spec
                    switch (data.Code)
                    {
                        case 0: // Bノッチ数
                            return data.SetValue(native.VehicleSpec.BrakeNotches);
                        case 1: // Pノッチ数
                            return data.SetValue(native.VehicleSpec.PowerNotches);
                        case 2: // ATS確認段
                            return data.SetValue(native.VehicleSpec.AtsNotch);
                        case 3: // B67相当段
                            return data.SetValue(native.VehicleSpec.B67Notch);
                        case 4: // 車両編成数
                            return data.SetValue(native.VehicleSpec.Cars);
                        default:
                            return data.SetError(Errors.ErrorInCodeNumber);
                    }
                case 'E':   // Status
                    switch (data.Code)
                    {
                        case 0: // 列車位置[m]
                            return data.SetValue((int)native.VehicleState.Location);
                        case 1: // 列車速度[km/h]
                            return data.SetValue((int)native.VehicleState.Speed);
                        case 2: // 現在時刻[ms]
                            return data.SetValue((int)native.VehicleState.Time.TotalMilliseconds);
                        case 3: // BC Pres[kPa]
                            return data.SetValue((int)native.VehicleState.BcPressure);
                        case 4: // MR Pres [kPa]
                            return data.SetValue((int)native.VehicleState.MrPressure);
                        case 5: // ER Pres [kPa]
                            return data.SetValue((int)native.VehicleState.ErPressure);
                        case 6: // BP Pres [kPa]
                            return data.SetValue((int)native.VehicleState.BpPressure);
                        case 7: // SAP Pres [kPa]
                            return data.SetValue((int)native.VehicleState.SapPressure);
                        case 8: // 電流 [A]
                            return data.SetValue((int)native.VehicleState.Current);
                        //case 9: // 電圧 [V]（準備工事）
                        //    return SetValue(hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch);
                        //    return response + hacker.Scenario.Vehicle.Instruments.Electricity.
                        case 10: // 現在時刻(HH)[時]
                            return data.SetValue(native.VehicleState.Time.Hours);
                        case 11: // 現在時刻(MM)[分]
                            return data.SetValue(native.VehicleState.Time.Minutes);
                        case 12: // 現在時刻(SS)[秒]
                            return data.SetValue(native.VehicleState.Time.Seconds);
                        case 13: // 現在時刻(ms)[ミリ秒]
                            return data.SetValue(native.VehicleState.Time.Milliseconds);
                        default:
                            return data.SetError(Errors.ErrorInCodeNumber);
                    }
                case 'H':   // Handle
                    switch (data.Code)
                    {
                        case 0: // Bノッチ位置
                            return data.SetValue(hacker.Scenario.Vehicle.Instruments.Cab.Handles.BrakeNotch);
                        case 1: // Pノッチ位置
                            return data.SetValue(hacker.Scenario.Vehicle.Instruments.Cab.Handles.PowerNotch);
                        case 2: // レバーサー位置
                            return data.SetValue((int)hacker.Scenario.Vehicle.Instruments.Cab.Handles.ReverserPosition);
                        case 3: // 定速状態（準備工事）
                            return data.SetValue((int)hacker.Scenario.Vehicle.Instruments.Cab.Handles.ConstantSpeedMode);
                        default:
                            return data.SetError(Errors.ErrorInCodeNumber);
                    }
                case 'P':   // Panel
                    try
                    {
                        return data.SetValue(native.AtsPanelArray.ElementAt(data.Code));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return data.SetError(Errors.OutOfRange);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return data.SetError(Errors.ErrorInCodeNumber);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return data.SetError(Errors.ErrorInCodeNumber);
                    }
                case 'S':   // Sound
                    try
                    {
                        return data.SetValue(native.AtsSoundArray.ElementAt(data.Code));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return data.SetError(Errors.OutOfRange);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return data.SetError(Errors.ErrorInCodeNumber);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return data.SetError(Errors.ErrorInCodeNumber);
                    }
                case 'D':   // ドア状態
                    switch (data.Code)
                    {
                        case 0:     // 全体
                            return data.SetValue(hacker.Scenario.Vehicle.Conductor.Doors.AreAllClosed);
                        case -1:    // 左（準備工事）
                        case 1:     // 右（準備工事）
                        default:
                            return data.SetError(Errors.ErrorInCodeNumber);
                    }
                default:
                    return data.SetError(Errors.ErrorInCodeSymbol);
            }
        }

        #endregion
    }
}
