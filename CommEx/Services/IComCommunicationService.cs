using CommEx.Models;

namespace CommEx.Services
{
    /// <summary>
    /// COM 通信ワーカー群を管理するサービス契約です。
    /// </summary>
    internal interface IComCommunicationService
    {
        void Start(ComTransportModel model);

        void PublishSnapshot(TelemetrySnapshot snapshot);

        void Stop();
    }
}
