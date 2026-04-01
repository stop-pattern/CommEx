using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO.Ports;

using BveExCsTemplate.CommEx.Settings.Models;

namespace BveExCsTemplate.CommEx.Settings.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        private TransportType selectedTransport = TransportType.Udp;

        public SettingsViewModel()
        {
            UdpNicSettings = new BindingList<UdpNicSetting>();
            ComPortSettings = new BindingList<ComPortSetting>();
            ApiServer = new ApiServerSetting();

            AvailableProtocols = Enum.GetValues(typeof(ComProtocolType)).Cast<ComProtocolType>().ToList();
            AvailableStopBits = new List<string> { "None", "One", "Two", "OnePointFive" };
            AvailableParity = new List<string> { "None", "Odd", "Even", "Mark", "Space" };
            AvailableComPorts = SerialPort.GetPortNames().OrderBy(name => name).ToList();

            LoadUdpInterfaces();
        }

        public BindingList<UdpNicSetting> UdpNicSettings { get; }

        public ApiServerSetting ApiServer { get; }

        public BindingList<ComPortSetting> ComPortSettings { get; }

        public IReadOnlyList<ComProtocolType> AvailableProtocols { get; }

        public IReadOnlyList<string> AvailableStopBits { get; }

        public IReadOnlyList<string> AvailableParity { get; }

        public IReadOnlyList<string> AvailableComPorts { get; }

        public TransportType SelectedTransport
        {
            get => selectedTransport;
            set
            {
                if (selectedTransport == value)
                {
                    return;
                }

                selectedTransport = value;
                RaisePropertyChanged();
            }
        }

        public void AddComPortSetting()
        {
            string portName = AvailableComPorts.FirstOrDefault() ?? "COM1";
            ComPortSettings.Add(new ComPortSetting { PortName = portName });
        }

        public void RemoveComPortSetting(int index)
        {
            if (index < 0 || index >= ComPortSettings.Count)
            {
                return;
            }

            ComPortSettings.RemoveAt(index);
        }

        private void LoadUdpInterfaces()
        {
            IEnumerable<NetworkInterface> interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up);

            foreach (NetworkInterface nic in interfaces)
            {
                IPInterfaceProperties properties = nic.GetIPProperties();
                IEnumerable<UnicastIPAddressInformation> ipAddresses = properties.UnicastAddresses
                    .Where(address => address.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Where(address => !IPAddress.IsLoopback(address.Address));

                foreach (UnicastIPAddressInformation ipAddress in ipAddresses)
                {
                    UdpNicSettings.Add(new UdpNicSetting
                    {
                        InterfaceName = nic.Name,
                        LocalIpAddress = ipAddress.Address.ToString(),
                    });
                }
            }
        }
    }
}
