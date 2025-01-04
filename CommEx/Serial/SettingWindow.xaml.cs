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


        //private void OpenButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // シリアルポートが既に開かれているか確認
        //    if (_serialPort != null && _serialPort.IsOpen)
        //    {
        //        PortClose();
        //    }
        //    else
        //    {
        //        PortOpen();
        //    }
        //}
    }
}
