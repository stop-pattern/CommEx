using System.Collections.Generic;

namespace CommEx.Models
{
    /// <summary>
    /// COM 通信全体の設定と状態を保持するモデルです。
    /// </summary>
    internal sealed class ComTransportModel
    {
        public bool Enabled { get; set; } = true;

        public IReadOnlyList<ComPortConfiguration> Ports { get; }

        public ComTransportModel(IReadOnlyList<ComPortConfiguration> ports)
        {
            Ports = ports;
        }
    }
}
