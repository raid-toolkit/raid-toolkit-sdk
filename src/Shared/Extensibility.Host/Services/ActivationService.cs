using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class ActivationServiceApi : ApiHandler<IActivationApi>, IActivationApi
    {
        private readonly IExtensionHostController ExtensionHostController;
        public ActivationServiceApi(IExtensionHostController extensionHostController, ILogger<ApiHandler<IActivationApi>> logger) : base(logger)
        {
            ExtensionHostController = extensionHostController;
        }

        public Task<bool> Activate(Uri activationRequestUri)
        {
            switch (activationRequestUri.Host)
            {
                case "extension":
                    ExtensionHost extensionHost = ExtensionHostController.GetExtensionPackageHost(activationRequestUri.LocalPath.TrimStart('/').Split('/')[0]);
                    return Task.FromResult(extensionHost.HandleRequest(activationRequestUri));
                default:
                    break;
            }
            return Task.FromResult(false);
        }
    }
}