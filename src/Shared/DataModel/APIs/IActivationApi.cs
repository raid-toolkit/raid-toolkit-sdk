using Newtonsoft.Json;

using Raid.Toolkit.Common;
using Raid.Toolkit.Common.API;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel
{
    [DeprecatedInV3]
    [PublicApi("activation-api")]
    public interface IActivationApi
    {
        [PublicApi("activate")]
        Task<int> Activate(Uri activationRequestUri, string[] arguments);
    }

    [DeprecatedInV3]
    public class ActivationApi : ApiBase<IActivationApi>, IActivationApi
    {
        public ActivationApi(RaidToolkitClientBase client) : base(client)
        {
        }

        public Task<int> Activate(Uri activationRequestUri, string[] arguments)
        {
            return CallMethod<int>(MethodBase.GetCurrentMethod()!, activationRequestUri, arguments);
        }
    }
}
