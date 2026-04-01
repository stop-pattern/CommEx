using System;
using System.Collections.Generic;

namespace BveExCsTemplate.Extension.Model
{
    /// <summary>
    /// BveEx から取得した 1 フレーム分の状態。
    /// </summary>
    internal sealed class SimulationFrame
    {
        public SimulationFrame(
            DateTimeOffset bveTimeUtc,
            bool isPaused,
            double simulationSpeed,
            IReadOnlyDictionary<string, object> inputs,
            IReadOnlyDictionary<string, object> outputs)
        {
            BveTimeUtc = bveTimeUtc;
            IsPaused = isPaused;
            SimulationSpeed = simulationSpeed;
            Inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
            Outputs = outputs ?? throw new ArgumentNullException(nameof(outputs));
        }

        public DateTimeOffset BveTimeUtc { get; }

        public bool IsPaused { get; }

        public double SimulationSpeed { get; }

        public IReadOnlyDictionary<string, object> Inputs { get; }

        public IReadOnlyDictionary<string, object> Outputs { get; }
    }
}
