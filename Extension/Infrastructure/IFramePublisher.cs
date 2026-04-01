using BveExCsTemplate.Extension.Model;

namespace BveExCsTemplate.Extension.Infrastructure
{
    internal interface IFramePublisher
    {
        void Publish(SimulationFrame frame);
    }
}
