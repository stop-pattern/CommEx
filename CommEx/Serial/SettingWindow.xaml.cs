using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;
using BveEx.Diagnostics;

namespace CommEx.Serial
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        /// <summary>
        /// シリアルポート
        /// </summary>
        private SerialPort _serialPort = new SerialPort();

        /// <summary>
        /// シリアルの制御
        /// </summary>
        private ISerialControl control = new Loopback();

        public SettingWindow()
        {
            InitializeComponent();

            // 使用可能なポート名をコンボボックスにセット
            foreach (string portName in SerialPort.GetPortNames())
            {
                PortNameComboBox.Items.Add(portName);
            }

            // 初期選択を設定（最初のポートを選択）
            if (PortNameComboBox.Items.Count > 0)
                PortNameComboBox.SelectedIndex = 0;

            // 初期設定としてデフォルト値を設定
            BaudRateComboBox.SelectedIndex = 4; // 115200
            DataBitsComboBox.SelectedIndex = 1; // 8
            StopBitsComboBox.SelectedIndex = 0; // 1
            ParityComboBox.SelectedIndex = 0; // None
            FlowControlComboBox.SelectedIndex = 0; // None
        }

        /// <summary>
        /// ポートを閉じる
        /// </summary>
        private void PortClose()
        {
            control.PortClose(_serialPort);
            _serialPort.Close();
            OpenButton.Content = "Open Port";
            //MessageBox.Show("Serial port closed.");

            // UIを有効化
            PortNameComboBox.IsEnabled = true;
            BaudRateComboBox.IsEnabled = true;
            DataBitsComboBox.IsEnabled = true;
            StopBitsComboBox.IsEnabled = true;
            ParityComboBox.IsEnabled = true;
            FlowControlComboBox.IsEnabled = true;
            PortStaus.Fill = new SolidColorBrush(Colors.Red);
        }

        /// <summary>
        /// ポートを開く
        /// </summary>
        private void PortOpen()
        {
            // ポート設定
            try
            {
                _serialPort.PortName = PortNameComboBox.Text;
                _serialPort.BaudRate = int.Parse(BaudRateComboBox.Text);
                _serialPort.DataBits = int.Parse(DataBitsComboBox.Text);
                _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBitsComboBox.Text);
                _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), ParityComboBox.Text);
                _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), FlowControlComboBox.Text);
                
                _serialPort = new SerialPort
                {
                    PortName = PortNameComboBox.Text,
                    BaudRate = int.Parse(BaudRateComboBox.Text),
                    DataBits = int.Parse(DataBitsComboBox.Text),
                    StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBitsComboBox.Text),
                    Parity = (Parity)Enum.Parse(typeof(Parity), ParityComboBox.Text),
                    Handshake = (Handshake)Enum.Parse(typeof(Handshake), FlowControlComboBox.Text)
                };
            }
            catch (Exception e)
            {
                ErrorDialog.Show(new ErrorDialogInfo("Serial setting error", e.Source, e.Message));
                return;
            }

            // ポートを開く
            try
            {
                // ポートを開く
                control.PortOpen(_serialPort);
                _serialPort.Open();
                OpenButton.Content = "Close Port";
                //MessageBox.Show("Serial port opened.");
            }
            catch (Exception e)
            {
                ErrorDialog.Show(new ErrorDialogInfo("Serial opening error", e.Source, e.Message));
                //MessageBox.Show($"Error opening serial port: {ex.Message}");
            }

            // UIを無効化
            PortNameComboBox.IsEnabled = false;
            BaudRateComboBox.IsEnabled = false;
            DataBitsComboBox.IsEnabled = false;
            StopBitsComboBox.IsEnabled = false;
            ParityComboBox.IsEnabled = false;
            FlowControlComboBox.IsEnabled = false;
            PortStaus.Fill = new SolidColorBrush(Colors.Green);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // シリアルポートが既に開かれているか確認
            if (_serialPort != null && _serialPort.IsOpen)
            {
                PortClose();
            }
            else
            {
                PortOpen();
            }
        }
    }
}
