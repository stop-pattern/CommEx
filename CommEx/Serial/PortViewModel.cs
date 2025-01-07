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

        /// <summary>
        /// シリアルポート
        /// </summary>
        private SerialPort port;

        /// <summary>
        /// シリアルの制御
        /// </summary>
        private ISerialControl control = new Loopback();

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
        public bool IsClosed
        {
            get
            {
                return !port.IsOpen;
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

        ///// <summary>
        ///// ポートを開く
        ///// </summary>
        //private void PortOpen()
        //{
        //    // ポート設定
        //    try
        //    {
        //        port.PortName = PortNameComboBox.Text;
        //        port.BaudRate = int.Parse(BaudRateComboBox.Text);
        //        port.DataBits = int.Parse(DataBitsComboBox.Text);
        //        port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBitsComboBox.Text);
        //        port.Parity = (Parity)Enum.Parse(typeof(Parity), ParityComboBox.Text);
        //        port.Handshake = (Handshake)Enum.Parse(typeof(Handshake), FlowControlComboBox.Text);
                
        //        port = new SerialPort
        //        {
        //            PortName = PortNameComboBox.Text,
        //            BaudRate = int.Parse(BaudRateComboBox.Text),
        //            DataBits = int.Parse(DataBitsComboBox.Text),
        //            StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBitsComboBox.Text),
        //            Parity = (Parity)Enum.Parse(typeof(Parity), ParityComboBox.Text),
        //            Handshake = (Handshake)Enum.Parse(typeof(Handshake), FlowControlComboBox.Text)
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorDialog.Show(new ErrorDialogInfo("Serial setting error", e.Source, e.Message));
        //        return;
        //    }

        //    // ポートを開く
        //    try
        //    {
        //        // ポートを開く
        //        control.PortOpen(port);
        //        port.Open();
        //        OpenButton.Content = "Close Port";
        //        //MessageBox.Show("Serial port opened.");
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorDialog.Show(new ErrorDialogInfo("Serial opening error", e.Source, e.Message));
        //        //MessageBox.Show($"Error opening serial port: {ex.Message}");
        //    }

        //    // UIを無効化
        //    PortNameComboBox.IsEnabled = false;
        //    BaudRateComboBox.IsEnabled = false;
        //    DataBitsComboBox.IsEnabled = false;
        //    StopBitsComboBox.IsEnabled = false;
        //    ParityComboBox.IsEnabled = false;
        //    FlowControlComboBox.IsEnabled = false;
        //    PortStaus.Fill = new SolidColorBrush(Colors.Green);
        //}

        ///// <summary>
        ///// ポートを閉じる
        ///// </summary>
        //private void PortClose()
        //{
        //    control.PortClose(port);
        //    port.Close();
        //    OpenButton.Content = "Open Port";
        //    //MessageBox.Show("Serial port closed.");

        //    // UIを有効化
        //    PortNameComboBox.IsEnabled = true;
        //    BaudRateComboBox.IsEnabled = true;
        //    DataBitsComboBox.IsEnabled = true;
        //    StopBitsComboBox.IsEnabled = true;
        //    ParityComboBox.IsEnabled = true;
        //    FlowControlComboBox.IsEnabled = true;
        //    PortStaus.Fill = new SolidColorBrush(Colors.Red);
        //}

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
