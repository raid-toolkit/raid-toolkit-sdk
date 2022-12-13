using Newtonsoft.Json;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel
{
    [PublicApi("activation-api")]
    public interface IActivationApi
    {
        [PublicApi("activate")]
        Task<int> Activate(Uri activationRequestUri, string[] arguments);
    }

    public class ActivationApi : ApiBase<IActivationApi>, IActivationApi
    {
        public ActivationApi(RaidToolkitClientBase client) : base(client)
        {
        }

        public Task<int> Activate(Uri activationRequestUri, string[] arguments)
        {
            return CallMethod<int>(MethodBase.GetCurrentMethod(), activationRequestUri, arguments);
        }
    }
}
