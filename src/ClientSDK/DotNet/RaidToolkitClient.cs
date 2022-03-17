using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class RaidToolkitClient
    {
        private readonly PromiseStore Promises = new();
        private readonly ClientWebSocket Socket = new();
        private readonly Uri EndpointUri;
        private readonly CancellationTokenSource CancellationTokenSource = new();

        public RaidToolkitClient(Uri endpointUri = null)
        {
            EndpointUri = endpointUri ?? new Uri("ws://localhost:9090");
        }

        public IAccountApi AccountApi => new AccountApi(this);
        public IStaticDataApi StaticDataApi => new StaticDataApi(this);
        public IRealtimeApi RealtimeApi => new RealtimeApi(this);
        private readonly Memory<byte> Buffer = new(new byte[1024 * 1024 * 16]);

        public static async Task EnsureInstalled()
        {
            if (!RegistrySettings.IsInstalled)
            {
                using var form = new Form { TopMost = true };
                var response = MessageBox.Show(
                    form,
                    "Raid Toolkit is required to be installed to access game data, would you like to download and install it now?",
                    "Installation required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );
                if (response != DialogResult.Yes)
                {
                    throw new NotSupportedException("Raid Toolkit must be installed");
                }
                try
                {
                    await InstallRTK();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(form, $"An error ocurred\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static async Task InstallRTK()
        {
            GitHub.Updater updater = new();
            GitHub.Schema.Release release = await updater.GetLatestRelease();
            if (release == null)
            {
                throw new FileNotFoundException("Could not find the latest release");
            }

            string tempFile = Path.Combine(Path.GetTempPath(), "Raid.Toolkit.exe");
            using (var stream = await updater.DownloadRelease(release))
            {
                using Stream newFile = File.Create(tempFile);
                stream.CopyTo(newFile);
            }
            Process proc = Process.Start(tempFile);
            // NET 472 doesn't support WaitForExitAsync
            proc.WaitForExit();
            // await proc.WaitForExitAsync();
        }

        public void Connect()
        {
            if (!RegistrySettings.IsInstalled)
            {
                throw new NotSupportedException("Raid Toolkit must be installed");
            }
            if (Socket.State == WebSocketState.None)
            {
                Socket.ConnectAsync(EndpointUri, CancellationToken.None).Wait();
                Listen();
            }
        }

        public void Disconnect()
        {
            CancellationTokenSource.Cancel();
            Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None).Wait();
        }

        private async void Listen()
        {
            while (Socket.State == WebSocketState.Open)
            {
                var segment = new ArraySegment<byte>(Buffer.ToArray());
                var result = await Socket.ReceiveAsync(segment, CancellationTokenSource.Token);
                if (!result.EndOfMessage)
                {
                    // TODO: throw away messages until next EndOfMessage is reached (inclusive)
                    continue;
                }
                var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(Encoding.UTF8.GetString(((Memory<byte>)segment).Slice(0, result.Count).Span.ToArray()));
                HandleMessage(socketMessage);
            }
        }

        private void HandleMessage(SocketMessage socketMessage)
        {
            switch (socketMessage.Channel)
            {
                case "set-promise":
                    {
                        Resolve(socketMessage.Message);
                        return;
                    }
                default:
                    break;
            }
        }

        private void Resolve(JToken message)
        {
            var promiseMsg = message.ToObject<PromiseMessage>();
            if (promiseMsg.Success)
            {
                Promises.Complete(promiseMsg.PromiseId, message.ToObject<PromiseSuccededMessage>().Value);
            }
            else
            {
                Promises.Fail(promiseMsg.PromiseId, message.ToObject<PromiseFailedMessage>().ErrorInfo);
            }
        }

        internal async void Subscribe(string apiName, string eventName)
        {
            await Send(new SocketMessage()
            {
                Scope = apiName,
                Channel = "sub",
                Message = JObject.FromObject(new SubscriptionMessage()
                {
                    EventName = eventName
                })
            });
        }

        internal async void Unsubscribe(string apiName, string eventName)
        {
            await Send(new SocketMessage()
            {
                Scope = apiName,
                Channel = "unsub",
                Message = JObject.FromObject(new SubscriptionMessage()
                {
                    EventName = eventName
                })
            });
        }

        internal async Task<T> Call<T>(string apiName, string methodName, params object[] args)
        {
            string promiseId = Promises.Create();
            await Send(new SocketMessage()
            {
                Scope = apiName,
                Channel = "call",
                Message = JObject.FromObject(new CallMethodMessage()
                {
                    PromiseId = promiseId,
                    MethodName = methodName,
                    Parameters = JArray.FromObject(args)
                })
            });
            return await Promises.GetTask<T>(promiseId);
        }

        private async Task Send(SocketMessage message)
        {
            await Socket.SendAsync(
                new(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)).AsMemory().ToArray()),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
