using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using BveExCsTemplate.CommEx.Settings.Models;
using BveExCsTemplate.CommEx.Settings.ViewModels;

namespace BveExCsTemplate.CommEx.Settings.Views
{
    internal class SettingsForm : Form
    {
        private readonly SettingsViewModel viewModel;
        private readonly ComboBox transportComboBox;
        private readonly Panel udpPanel;
        private readonly Panel apiPanel;
        private readonly Panel comPanel;

        public SettingsForm(SettingsViewModel viewModel)
        {
            this.viewModel = viewModel;

            Text = "CommEx 設定";
            Width = 1000;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            transportComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top,
                Height = 32,
            };
            transportComboBox.DataSource = Enum.GetValues(typeof(TransportType));
            transportComboBox.SelectedItem = viewModel.SelectedTransport;
            transportComboBox.SelectedIndexChanged += TransportComboBoxOnSelectedIndexChanged;

            udpPanel = BuildUdpPanel();
            apiPanel = BuildApiPanel();
            comPanel = BuildComPanel();

            Controls.Add(comPanel);
            Controls.Add(apiPanel);
            Controls.Add(udpPanel);
            Controls.Add(transportComboBox);

            UpdateVisiblePanel();
        }

        private Panel BuildUdpPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            var udpGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                DataSource = new BindingSource { DataSource = viewModel.UdpNicSettings },
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
            };

            udpGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(UdpNicSetting.InterfaceName),
                HeaderText = "NIC",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            });

            udpGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(UdpNicSetting.LocalIpAddress),
                HeaderText = "自身のIPアドレス",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            });

            udpGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(UdpNicSetting.DestinationIpAddress),
                HeaderText = "宛先IPアドレス",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            });

            panel.Controls.Add(udpGrid);
            return panel;
        }

        private Panel BuildApiPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            var portLabel = new Label
            {
                Text = "APIサーバーのポート番号",
                AutoSize = true,
                Location = new Point(20, 24),
            };

            var portInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 65535,
                Value = viewModel.ApiServer.Port,
                Location = new Point(20, 50),
                Width = 140,
            };
            portInput.ValueChanged += (_, __) => viewModel.ApiServer.Port = Decimal.ToInt32(portInput.Value);

            panel.Controls.Add(portLabel);
            panel.Controls.Add(portInput);
            return panel;
        }

        private Panel BuildComPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
            };

            var addButton = new Button { Text = "ポート追加", Width = 110 };
            addButton.Click += (_, __) => viewModel.AddComPortSetting();

            var removeButton = new Button { Text = "選択ポート削除", Width = 130 };

            var comGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                DataSource = new BindingSource { DataSource = viewModel.ComPortSettings },
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
            };

            removeButton.Click += (_, __) => viewModel.RemoveComPortSetting(comGrid.CurrentCell?.RowIndex ?? -1);

            comGrid.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.Protocol),
                HeaderText = "プロトコル",
                DataSource = viewModel.AvailableProtocols.ToList(),
            });

            comGrid.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.PortName),
                HeaderText = "COMポート",
                DataSource = viewModel.AvailableComPorts.ToList(),
            });

            comGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.BaudRate),
                HeaderText = "ボーレート",
            });

            comGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.DataBits),
                HeaderText = "データビット",
            });

            comGrid.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.StopBits),
                HeaderText = "ストップビット",
                DataSource = viewModel.AvailableStopBits.ToList(),
            });

            comGrid.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.Parity),
                HeaderText = "パリティ",
                DataSource = viewModel.AvailableParity.ToList(),
            });

            comGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.DtrEnable),
                HeaderText = "DTR",
            });

            comGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.RtsEnable),
                HeaderText = "RTS",
            });

            comGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(ComPortSetting.AutoStart),
                HeaderText = "自動起動",
            });

            topPanel.Controls.Add(addButton);
            topPanel.Controls.Add(removeButton);

            panel.Controls.Add(comGrid);
            panel.Controls.Add(topPanel);

            return panel;
        }

        private void TransportComboBoxOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (transportComboBox.SelectedItem is TransportType selected)
            {
                viewModel.SelectedTransport = selected;
                UpdateVisiblePanel();
            }
        }

        private void UpdateVisiblePanel()
        {
            udpPanel.Visible = viewModel.SelectedTransport == TransportType.Udp;
            apiPanel.Visible = viewModel.SelectedTransport == TransportType.ApiServer;
            comPanel.Visible = viewModel.SelectedTransport == TransportType.ComPort;
        }
    }
}
