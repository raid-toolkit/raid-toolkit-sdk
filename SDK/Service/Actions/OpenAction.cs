using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raid.DataModel;

namespace Raid.Service
{
    [Verb("open", HelpText = "Opens a RTK link")]
    public class OpenOptions
    {
        [Value(0, MetaName = "uri", HelpText = "Uri", Required = false)]
        public string Uri { get; set; }
    }

    static class OpenAction
    {
        private static bool IsRunning
        {
            get
            {
                using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
                {
                    bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                    if (!isAnotherInstanceOpen)
                    {
                        mutex.ReleaseMutex();
                    }
                    return isAnotherInstanceOpen;
                }
            }
        }

        public static async Task<int> Execute(OpenOptions options)
        {
            if (!IsRunning)
            {
                Process.Start(AppConfiguration.ExecutablePath);
            }

            if (string.IsNullOrEmpty(options.Uri))
            {
                return 0;
            }

            int portNumber = AppConfiguration.Configuration.GetValue<int>("serverOptions:listeners:0:port");
            ClientWebSocket socket = new();
            await socket.ConnectAsync(new Uri($"ws://localhost:{portNumber}"), CancellationToken.None);

            SocketMessage msg = new()
            {
                Scope = "$rtk",
                Channel = "open",
                Message = JObject.FromObject(options)
            };

            await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)).AsMemory(), WebSocketMessageType.Text, true, CancellationToken.None).AsTask();
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
            return 0;
        }
    }
}