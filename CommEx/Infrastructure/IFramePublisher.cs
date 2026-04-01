using CommEx.Model;

namespace CommEx.Infrastructure
{
    internal interface IFramePublisher
    {
        void Publish(SimulationFrame frame);
    }
}
