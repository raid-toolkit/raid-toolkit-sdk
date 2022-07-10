using System;
using System.Diagnostics;
using Client.Model;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility.Host
{
    public class GameInstance : IGameInstance, IDisposable
    {
        public int Token { get; }
        public string Id { get; }
        private readonly string AccountName;
        private bool IsDisposed;

        public Il2CsRuntimeContext Runtime { get; }
        public PropertyBag Properties { get; } = new();

        public GameInstance(Process proc)
        {
            Token = proc.Id;
            Runtime = new(proc);
            (Id, AccountName) = GetAccountIdAndName();
        }

        private (string, string) GetAccountIdAndName()
        {
            var appModel = Client.App.SingleInstance<AppModel>._instance.GetValue(Runtime);

            var userWrapper = appModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            return (string.Join('_', globalId, socialId).Sha256(), userWrapper.UserGameSettings.GameSettings.Name);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Runtime?.Dispose();
                }
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
