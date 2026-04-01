namespace BveExCsTemplate.CommEx.Settings.Models
{
    internal class ComPortSetting
    {
        public ComProtocolType Protocol { get; set; } = ComProtocolType.Binary;

        public string PortName { get; set; } = "COM1";

        public int BaudRate { get; set; } = 115200;

        public int DataBits { get; set; } = 8;

        public string StopBits { get; set; } = "One";

        public string Parity { get; set; } = "None";

        public bool DtrEnable { get; set; }

        public bool RtsEnable { get; set; }

        public bool AutoStart { get; set; }
    }
}
