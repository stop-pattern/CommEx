using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace CommEx.Serial.Common
{
    #region Enum

    /// <summary>
    /// 改行文字
    /// </summary>
    [TypeConverter(typeof(NewLinesConverter))]
    public enum NewLines
    {
        [Description("\n")]
        LF,
        [Description("\r\n")]
        CRLF,
        [Description("\r")]
        CR
    }

    #endregion

    #region TypeConverter

    // https://qiita.com/mkuwan/items/be6745f2c9e7823f8a66
    internal static class EnumExtention
    {
        /// <summary>
        /// Enum の Value から Description を取得
        /// </summary>
        /// <typeparam name="T">任意の <see cref="Enum"/></typeparam>
        /// <param name="value">型が T の値</param>
        /// <returns>value の Description</returns>
        internal static string GetDescriptionFromValue<T>(this T value) where T : Enum //where T : Enum とすることで Tがenumでない場合はコンパイル時にエラーにしてくれる
        {
            //valueはenum型確定なので空文字が返ることはない
            string strValue = value.ToString();

            var description =
                typeof(T).GetField(strValue)    //FiledInfoを取得
                .GetCustomAttributes(typeof(DescriptionAttribute), false)   //DescriptionAttributeのリストを取得
                .Cast<DescriptionAttribute>()   //DescriptionAttributeにキャスト
                .FirstOrDefault()               //最初の一つを取得、なければnull
                ?.Description;                  //DescriptionAttributeがあればDescriptionを、なければnullを返す

            return description ?? strValue;     //descriptionがnullならstrValueを返す
        }

        /// <summary>
        /// EnumのDescriptionからValueを取得
        /// </summary>
        /// <typeparam name="T">任意の <see cref="Enum"/></typeparam>
        /// <param name="description">型が T の value の Description</param>
        /// <returns>Description を属性に持つ型が T の値</returns>
        internal static T GetEnumValueFromDescription<T>(this string description) where T : Enum
        {
            var value =
                typeof(T).GetFields()
                .SelectMany(x => x.GetCustomAttributes(typeof(DescriptionAttribute), false),
                    (f, a) => new { field = f, attribute = a })
                .Where(x => ((DescriptionAttribute)x.attribute).Description == description)
                .FirstOrDefault()
                ?.field.GetRawConstantValue();

            // 値が見つからない場合にエラーとする場合はこちら
            //return (T)(value ?? throw new ArgumentNullException());

            return (T)(value ?? default(T));
        }
    }


    internal class NewLinesConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = (string)value;
                switch (str)
                {
                    case "CRLF":
                        return NewLines.CRLF;
                    case "CR":
                        return NewLines.CR;
                    case "LF":
                    default:
                        return NewLines.LF;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                switch ((NewLines)value)
                {
                    case NewLines.CRLF:
                        return "CRLF";
                    case NewLines.CR:
                        return "CR";
                    case NewLines.LF:
                    default:
                        return "LF";
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion
}
