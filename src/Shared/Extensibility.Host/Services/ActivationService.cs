using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class ActivationServiceApi : ApiHandler<IActivationApi>, IActivationApi
    {
        public ActivationServiceApi(ILogger<ApiHandler<IActivationApi>> logger) : base(logger)
        {
        }

        public Task<bool> Activate(Uri activationRequestUri)
        {
            return Task.FromResult(false);
        }
    }
}