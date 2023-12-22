using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class ActivationServiceApi : ApiHandler<IActivationApi>, IActivationApi
    {
        private readonly IPackageManager PackageManager;
        private readonly IExtensionHostController ExtensionHostController;
        public ActivationServiceApi(IExtensionHostController extensionHostController, IPackageManager packageManager, ILogger<ApiHandler<IActivationApi>> logger) : base(logger)
        {
            ExtensionHostController = extensionHostController;
            PackageManager = packageManager;
        }

        public async Task<int> Activate(Uri activationRequestUri, string[] arguments)
        {
            switch (activationRequestUri.Host)
            {
                case "install-extension":
                    {
                        ExtensionBundle bundle = ExtensionBundle.FromFile(arguments[0]);
                        await PackageManager.RequestPackageInstall(bundle);
                        return 0;
                    }
                case "extension":
                    {
                        string extensionId = activationRequestUri.LocalPath.TrimStart('/').Split('/')[0];
                        if (!ExtensionHostController.TryGetExtension(extensionId, out IExtensionManagement? extensionHost))
                            throw new ApplicationException($"Extension '{extensionId}' not found.");

                        return extensionHost.HandleRequest(activationRequestUri);
                    }
                default:
                    break;
            }
            return -1;
        }
    }
}
