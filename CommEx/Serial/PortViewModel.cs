using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommEx.Serial
{
    internal class PortViewModel : INotifyPropertyChanged
    {
        #region Fields

        private SerialPort port;

        #endregion

        #region Properties

        /// <summary>
        /// ボーレート[Baud]
        /// </summary>
        [Browsable(true)]
        [DefaultValue(9600)]
        [MonitoringDescription("BaudRate")]
        public int BaudRate
        {
            get
            {
                return port.BaudRate;
            }
            set
            {
                port.BaudRate = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// データビット[bit]
        /// </summary>
        [Browsable(true)]
        [DefaultValue(8)]
        [MonitoringDescription("DataBits")]
        public int DataBits
        {
            get
            {
                return port.DataBits;
            }
            set
            {
                port.DataBits = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DTR 有効/無効
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [MonitoringDescription("DtrEnable")]
        public bool DtrEnable
        {
            get
            {
                return port.DtrEnable;
            }
            set
            {
                port.DtrEnable = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// テキストのエンコーディング
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [MonitoringDescription("Encoding")]
        public Encoding Encoding
        {
            get
            {
                return port.Encoding;
            }
            set
            {
                port.Encoding = value;
                RaisePropertyChanged();;
            }
        }

        /// <summary>
        /// フロー制御
        /// </summary>
        [Browsable(true)]
        [DefaultValue(Handshake.None)]
        [MonitoringDescription("Handshake")]
        public Handshake Handshake
        {
            get
            {
                return port.Handshake;
            }
            set
            {
                port.Handshake = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ポートの状態
        /// </summary>
        [Browsable(false)]
        public bool IsOpen
        {
            get
            {
                return port.IsOpen;
            }
        }

        /// <summary>
        /// 改行文字
        /// </summary>
        [Browsable(false)]
        [DefaultValue("\n")]
        [MonitoringDescription("NewLine")]
        public string NewLine
        {
            get
            {
                return port.NewLine;
            }
            set
            {
                port.NewLine = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// パリティ
        /// </summary>
        [Browsable(true)]
        [DefaultValue(Parity.None)]
        [MonitoringDescription("Parity")]
        public Parity Parity
        {
            get
            {
                return port.Parity;
            }
            set
            {
                port.Parity = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ポート名
        /// </summary>
        [Browsable(true)]
        [DefaultValue("COM1")]
        [MonitoringDescription("PortName")]
        public string PortName
        {
            get
            {
                return port.PortName;
            }
            set
            {
                port.PortName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ストップビット[bit]
        /// </summary>
        [Browsable(true)]
        [DefaultValue(StopBits.One)]
        [MonitoringDescription("StopBits")]
        public StopBits StopBits
        {
            get
            {
                return port.StopBits;
            }
            set
            {
                port.StopBits = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Methods 

        /// <summary>
        /// ViewModel をデフォルト値で初期化
        /// </summary>
        public PortViewModel()
        {
            port = new SerialPort();
        }

        /// <summary>
        /// ViewModel を値を指定して初期化
        /// </summary>
        /// <param name="portName">ポート名</param>
        /// <param name="baudRate">ボーレート</param>
        /// <param name="parity">パリティ</param>
        /// <param name="dataBits">データビット</param>
        /// <param name="stopBits">ストップビット</param>
        public PortViewModel(string portName = "COM0", int baudRate = 115200, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }

        /// <summary>
        /// ViewModel を <see cref="SerialPort"/> で初期化
        /// </summary>
        /// <param name="serialPort">初期化に使用する <see cref="SerialPort"/></param>
        public PortViewModel(SerialPort serialPort)
        {
            port = serialPort;
        }

        #endregion

        #region Interface Implementation

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// View に値の変更を通知
        /// </summary>
        /// <param name="propertyName">呼び出し元のプロパティ名（自動取得）</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
