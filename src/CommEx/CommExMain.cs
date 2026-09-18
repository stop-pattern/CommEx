using System;
using BveEx.PluginHost.Plugins;
using BveEx.PluginHost.Plugins.Extensions;

namespace CommEx
{
    /// <summary>
    /// Provides the minimal BveEX extension entry point for development and host-load verification.
    /// </summary>
    /// <remarks>This bootstrap owns no workers, events, UI, files or communication resources.</remarks>
    [Plugin(PluginType.Extension)]
    public sealed class CommExMain : AssemblyPluginBase, IExtension
    {
        /// <summary>
        /// Initializes the extension metadata and host services supplied by BveEX.
        /// </summary>
        /// <param name="builder">The valid plugin construction context supplied by the BveEX loader.</param>
        public CommExMain(PluginBuilder builder)
            : base(builder)
        {
        }

        /// <summary>
        /// Accepts a host update without changing simulation state or performing I/O.
        /// </summary>
        /// <param name="elapsed">The interval since the preceding host update.</param>
        public override void Tick(TimeSpan elapsed)
        {
        }

        /// <summary>
        /// Completes disposal; this bootstrap owns no resources and repeated calls have no effect.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
