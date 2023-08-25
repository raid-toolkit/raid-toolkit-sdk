using System;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Realtime
{
    public class RealtimeExtension : ExtensionPackage, IDisposable
    {
        public RealtimeExtension(ILogger<RealtimeExtension> logger)
        {
        }

        public override void OnActivate(IExtensionHost host)
        {
            Disposables.Add(host.RegisterMessageScopeHandler<RealtimeApi>(host.CreateInstance<RealtimeApi>(host)));
            Disposables.Add(host.RegisterBackgroundService<RealtimeService>());
        }
    }
}
