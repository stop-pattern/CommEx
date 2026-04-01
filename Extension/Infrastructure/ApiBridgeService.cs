using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using BveExCsTemplate.Extension.Model;

namespace BveExCsTemplate.Extension.Infrastructure
{
    /// <summary>
    /// BveEx の入出力をそのまま HTTP API で公開する。
    /// </summary>
    internal sealed class ApiBridgeService : IDisposable
    {
        private readonly HttpListener listener;
        private readonly BveExModelStore model;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private Task loopTask;

        public ApiBridgeService(BveExModelStore model, string prefix)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            if (listener.IsListening) return;
            listener.Start();
            loopTask = Task.Run((Func<Task>)AcceptLoopAsync);
        }

        private async Task AcceptLoopAsync()
        {
            while (!cts.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                _ = Task.Run(() => Handle(context));
            }
        }

        private void Handle(HttpListenerContext context)
        {
            var frame = model.GetCurrentFrame();
            var path = context.Request.Url.AbsolutePath;

            if (path.Equals("/api/state", StringComparison.OrdinalIgnoreCase))
            {
                WriteJson(context.Response, BuildStatePayload(frame));
                return;
            }

            if (path.StartsWith("/api/inputs/", StringComparison.OrdinalIgnoreCase))
            {
                var key = path.Substring("/api/inputs/".Length);
                WriteJson(context.Response, BuildValuePayload(frame.Inputs, key, "input"));
                return;
            }

            if (path.StartsWith("/api/outputs/", StringComparison.OrdinalIgnoreCase))
            {
                var key = path.Substring("/api/outputs/".Length);
                WriteJson(context.Response, BuildValuePayload(frame.Outputs, key, "output"));
                return;
            }

            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        internal object BuildStatePayload(SimulationFrame frame)
        {
            return new
            {
                bveTimeUtc = frame.BveTimeUtc.UtcDateTime.ToString("O"),
                isPaused = frame.IsPaused,
                simulationSpeed = frame.SimulationSpeed,
                inputs = frame.Inputs,
                outputs = frame.Outputs,
            };
        }

        internal object BuildValuePayload(System.Collections.Generic.IReadOnlyDictionary<string, object> source, string key, string type)
        {
            object value;
            if (source.TryGetValue(key, out value))
            {
                return new { name = key, kind = type, value };
            }

            return new { name = key, kind = type, value = (object)null };
        }

        private void WriteJson(HttpListenerResponse response, object payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            var bytes = Encoding.UTF8.GetBytes(json);
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = bytes.Length;
            using (var stream = response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            if (listener.IsListening)
            {
                listener.Stop();
            }
            listener.Close();
            if (loopTask != null)
            {
                try
                {
                    loopTask.Wait(TimeSpan.FromSeconds(1));
                }
                catch (AggregateException)
                {
                }
            }
            cts.Dispose();
        }
    }
}
