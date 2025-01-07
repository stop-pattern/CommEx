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
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using BveEx.Diagnostics;

namespace CommEx.Serial
{
    public class PortViewModel : INotifyPropertyChanged
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
                RaisePropertyChanged();
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
        [Browsable(true)]
        [DefaultValue(Handshake.None)]
        [MonitoringDescription("IsOpen")]
        public bool IsOpen
        {
            get
            {
                return port.IsOpen;
            }
        }

        /// <summary>
        /// ポートの状態
        /// </summary>
        [Browsable(false)]
        public bool IsClosed => !IsOpen;

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

        /// <summary>
        /// 使用可能なポートの選択肢リスト
        /// </summary>
        public ObservableCollection<string> AvailablePorts { get; } = new ObservableCollection<string>();

        /// <summary>
        /// ボーレートの選択肢リスト
        /// </summary>
        public ObservableCollection<int> BaudRates { get; } = new ObservableCollection<int> { 9600, 19200, 38400, 57600, 115200 };

        /// <summary>
        /// データビットの選択肢リスト
        /// </summary>
        public ObservableCollection<int> DataBitsOptions { get; } = new ObservableCollection<int> { 5, 6, 7, 8 };

        /// <summary>
        /// ストップビットの選択肢リスト
        /// </summary>
        public ObservableCollection<StopBits> StopBitsOptions { get; } = new ObservableCollection<StopBits> { StopBits.One, StopBits.OnePointFive, StopBits.Two };

        /// <summary>
        /// パリティの選択肢リスト
        /// </summary>
        public ObservableCollection<Parity> ParityOptions { get; } = new ObservableCollection<Parity>(Enum.GetValues(typeof(Parity)) as Parity[]);

        /// <summary>
        /// フロー制御の選択肢リスト
        /// </summary>
        public ObservableCollection<Handshake> HandshakeOptions { get; } = new ObservableCollection<Handshake>(Enum.GetValues(typeof(Handshake)) as Handshake[]);

        /// <summary>
        /// ポートリストのアップデートコマンド
        /// </summary>
        public ICommand UpdatePortsCommand { get; }

        /// <summary>
        /// ポートの開閉コマンド
        /// </summary>
        public ICommand OpenClosePortCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// ViewModel をデフォルト値で初期化
        /// </summary>
        public PortViewModel()
        {
            port = new SerialPort();

            UpdatePortsCommand = new RelayCommand(UpdatePorts);
            OpenClosePortCommand = new RelayCommand(OpenClosePort, CanOpenClosePort);

            //UpdatePorts();
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

            UpdatePortsCommand = new RelayCommand(UpdatePorts);
            OpenClosePortCommand = new RelayCommand(OpenClosePort, CanOpenClosePort);

            //UpdatePorts();
        }

        /// <summary>
        /// ViewModel を <see cref="SerialPort"/> で初期化
        /// </summary>
        /// <param name="serialPort">初期化に使用する <see cref="SerialPort"/></param>
        public PortViewModel(SerialPort serialPort)
        {
            port = serialPort;

            UpdatePortsCommand = new RelayCommand(UpdatePorts);
            OpenClosePortCommand = new RelayCommand(OpenClosePort, CanOpenClosePort);

            //UpdatePorts();
        }

        /// <summary>
        /// ポートの開閉が可能か否か判定
        /// </summary>
        /// <returns>ポート操作可否</returns>
        private bool CanOpenClosePort() => port != null && !string.IsNullOrEmpty(PortName);

        /// <summary>
        /// ポートリストのアップデート
        /// </summary>
        private void UpdatePorts()
        {
            AvailablePorts.Clear();
            foreach (var port in SerialPort.GetPortNames())
            {
                AvailablePorts.Add(port);
            }
        }

        /// <summary>
        /// ポートの開閉
        /// </summary>
        private void OpenClosePort()
        {
            if (!IsOpen)
            {
                // ポートを開ける
                try
                {
                    // control.PortOpen(port);
                    port.Open();
                }
                catch (UnauthorizedAccessException ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("ポートが既に使われています。", ex.Source, ex.Message));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("ポートの設定が無効です。", ex.Source, ex.Message));
                }
                catch (ArgumentException ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("このポートはサポートされていません。", ex.Source, ex.Message));
                }
                catch (IOException ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("ポートが無効状態です。", ex.Source, ex.Message));
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("ポートのオープンに失敗しました。", ex.Source, ex.Message));
                }
            }
            else
            {
                // ポートを閉じる
                try
                {
                    port.Close();
                    // control.PortClose(port);
                }
                catch (IOException ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("ポートが無効状態です。", ex.Source, ex.Message));
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show(new ErrorDialogInfo("ポートを閉じたときにエラーが発生しました。", ex.Source, ex.Message));
                }
            }
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

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
