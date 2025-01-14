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
using CommEx.Serial.ViewModels;

namespace CommEx.Serial.Views
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            DataContext = new PortViewModel();
        }

        public SettingWindow(PortViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void DropDownOpened(object sender, EventArgs e)
        {
            if (DataContext is PortViewModel viewModel)
            {
                viewModel.UpdatePortsCommand.Execute(null);
            }
        }
    }
}
