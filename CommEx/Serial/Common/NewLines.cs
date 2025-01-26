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
    /// <summary>
    /// 改行文字
    /// </summary>
    [TypeConverter(typeof(EnumToStringConverter))]
    public enum NewLines
    {
        [Description("\n")]
        LF,
        [Description("\r\n")]
        CRLF,
        [Description("\r")]
        CR
    }
}
