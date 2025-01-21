using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CommEx.Serial.Common
{
    /// <summary>
    /// bool 型を Color に変換するコンバータ
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOpen)
            {
                return isOpen ? Brushes.Green : Brushes.Red;
            }

            return Brushes.Gray; // デフォルト色
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush brush)
            {
                if (brush == Brushes.Green)
                {
                    return true;
                }
                else if (brush == Brushes.Red)
                {
                    return false;
                }
                else if (brush == Brushes.Gray)
                {
                    return false;
                }
            }

            throw new InvalidOperationException("Unsupported conversion");
        }
    }
}
