using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommEx.Serial.Common
{
    /// <summary>
    /// シリアル通信の制御器
    /// </summary>
    [TypeConverter(typeof(EnumToDescriptionConverter))]
    public enum Controller
    {
        [Description("Loopback")]
        Loopback,
        [Description("BIDS互換")]
        Bids,
    }

    /// <inheritdoc/>
    /// <summary>
    /// Controller とその ISerialControl を変換するコンバータ
    /// </summary>
    public class ControllerToISerialControlConverter : EnumToDescriptionConverter
    {
        public ControllerToISerialControlConverter(Type type) : base(type)
        {
            if (type != typeof(Controller))
            {
                throw new ArgumentException($"Type must be {typeof(Controller)}.");
            }
        }

        /// <summary>
        /// <see cref="Controller"/> の value に応じたクラス
        /// </summary>
        private static readonly Dictionary<Controller, Type> ClassDictionary = new Dictionary<Controller, Type>
            {
            //  { <Enum.Value>, typeof(<制御クラス>) },
                { Controller.Loopback, typeof(Loopback) },
                { Controller.Bids, typeof(Bids.BidsSerial) }
            };

        /// <summary>
        /// Enum の Value から ISerialControl のインスタンスを取得
        /// Enum.Value -> ISerialControl
        /// </summary>
        /// <param name="value">Enum.value</param>
        /// <param name="destinationType">ISerialControl</param>
        /// <returns>ISerialControl</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
            {
                Debug.WriteLine("value が null です。");
                return null;
            }

            if (destinationType == typeof(ISerialControl))
            {

                if (ClassDictionary.TryGetValue((Controller)value, out Type type))
                {
                    return (ISerialControl)Activator.CreateInstance(type);
                }
                else
                {
                    Debug.WriteLine("クラスが登録されていません。");
                    return null;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// ISerialControl のインスタンスから Enum の Value を取得
        /// ISerialControl -> Enum.Value
        /// </summary>
        /// <param name="value">ISerialControl</param>
        /// <returns>Enum.value</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                Debug.WriteLine("value が null です。");
                return null;
            }

            if (value is ISerialControl)
            {
                // controllerの型をSerialControl.Controllerに変換
                // 現在のcontrollerインスタンスがClassDictionaryのどのキーに対応するかを探す
                foreach (var item in ClassDictionary)
                {
                    if (item.Value == value.GetType())
                    {
                        return item.Key;
                    }
                }
                Debug.WriteLine("クラスが登録されていません。");
                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

}
