using Client.Model;
using Il2CppToolkit.Runtime;
using Raid.Toolkit.Common;
using Raid.Toolkit.Injection;
using System;
using System.Diagnostics;

namespace Raid.Toolkit.Extensibility.Host
{
    public class GameInstance : IGameInstance, IDisposable
    {
        public int Token { get; }
        public string Id { get; }
        private readonly string AccountName;
        private bool IsDisposed;
        private ProcessHook Hook;

        public Il2CsRuntimeContext Runtime { get; }
        public PropertyBag Properties { get; } = new();

        public GameInstance(Process proc)
        {
            Token = proc.Id;
            Runtime = new(proc);
            if (RegistrySettings.IsFlagEnabled(FeatureFlags.Hooks, true))
            {
                Hook = new(proc.Id);
            }
            (Id, AccountName) = GetAccountIdAndName();
        }

        private (string, string) GetAccountIdAndName()
        {
            var appModel = Client.App.SingleInstance<AppModel>.method_get_Instance
                        .GetMethodInfo(Runtime).DeclaringClass.StaticFields
                        .As<SingleInstanceStaticFields<AppModel>>().Instance;

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
                    Hook?.Dispose();
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
