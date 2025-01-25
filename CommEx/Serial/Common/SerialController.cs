using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommEx.Serial.Common
{
    /// <summary>
    /// シリアル通信の制御方法
    /// </summary>
    [TypeConverter(typeof(EnumToDescriptionConverter))]
    enum SerialController
    {
        [Description("Loopback")]
        Loopback,
        [Description("BIDS")]
        Bids,
    }
}
