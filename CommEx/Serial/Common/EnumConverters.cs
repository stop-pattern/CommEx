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
    #region StringConverter

    /// <inheritdoc/>
    /// <summary>
    /// Enum の Value と Description を変換するコンバータ
    /// </summary>
    public class EnumToStringConverter : EnumConverter
    {
        public EnumToStringConverter(Type type) : base(type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type must be an Enum.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse(EnumType, stringValue);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    #endregion

    #region DescriptionConverter

    /// <inheritdoc/>
    /// <summary>
    /// Enum の Value と Description を変換するコンバータ
    /// </summary>
    public class EnumToDescriptionConverter : EnumConverter
    {
        public EnumToDescriptionConverter(Type type) : base(type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type must be an Enum.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                var fieldInfo = value.GetType().GetField(value.ToString());
                var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    return descriptionAttribute.Description;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                foreach (var field in EnumType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
                    if (descriptionAttribute != null && descriptionAttribute.Description == stringValue)
                    {
                        return Enum.Parse(EnumType, field.Name);
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    #endregion
}
