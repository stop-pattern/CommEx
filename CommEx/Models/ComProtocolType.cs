namespace CommEx.Models
{
    /// <summary>
    /// COM ポートごとに選択可能な通信プロトコル種別です。
    /// </summary>
    internal enum ComProtocolType
    {
        Bids = 0,
        CommunicationDll = 1,
        BveSerialOutput = 2,
        CustomBinary = 3,
    }
}
