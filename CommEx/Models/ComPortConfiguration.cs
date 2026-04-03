using System.IO.Ports;

namespace CommEx.Models
{
    /// <summary>
    /// 1 つの COM ポート設定を表すモデルです。
    /// </summary>
    internal sealed class ComPortConfiguration
    {
        public string PortName { get; set; } = "COM1";

        public ComProtocolType ProtocolType { get; set; } = ComProtocolType.Bids;

        public int BaudRate { get; set; } = 115200;

        public int DataBits { get; set; } = 8;

        public StopBits StopBits { get; set; } = StopBits.One;

        public Parity Parity { get; set; } = Parity.None;

        public bool DtrEnable { get; set; } = true;

        public bool RtsEnable { get; set; }

        public bool AutoStart { get; set; } = true;
    }
}
