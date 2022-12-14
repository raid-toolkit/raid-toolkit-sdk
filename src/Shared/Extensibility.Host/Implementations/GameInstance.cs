using System;
using System.Diagnostics;
using Client.Model;
using Il2CppToolkit.Common.Errors;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility.Host
{
    public class GameInstance : IGameInstance, IDisposable
    {
        public int Token { get; private set; }
        public string Id { get; private set; }
        public string AccountName { get; private set; }
        public string AvatarUrl { get; private set; }
        private bool IsDisposed;

        public Il2CsRuntimeContext Runtime { get; private set; }
        public PropertyBag Properties { get; } = new();

        public GameInstance(Process proc)
        {
            Token = proc.Id;
        }

        public void InitializeOrThrow(Process proc)
        {
            Runtime ??= new(proc);

            ErrorHandler.VerifyElseThrow(Runtime != null, ServiceError.MethodCalledBeforeInitialization, "Method cannot be called before intialization");
            var appModel = Client.App.SingleInstance<AppModel>._instance.GetValue(Runtime);

            var userWrapper = appModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            AvatarUrl = $"https://raid-toolkit.github.io/img/avatars/{(int)userWrapper.UserGameSettings.GameSettings.Avatar}.png";
            Id = string.Join('_', globalId, socialId).Sha256();
            AccountName = userWrapper.UserGameSettings.GameSettings.Name;
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
