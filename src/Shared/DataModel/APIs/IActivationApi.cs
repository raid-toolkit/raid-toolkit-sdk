using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel
{
    [PublicApi("activation-api")]
    public interface IActivationApi
    {
        [PublicApi("activate")]
        Task<bool> Activate(Uri activationRequestUri);
    }

    public class ActivationApi : ApiBase<IActivationApi>, IActivationApi
    {
        public ActivationApi(RaidToolkitClientBase client) : base(client)
        {
        }

        public Task<bool> Activate(Uri activationRequestUri)
        {
            return CallMethod<bool>(MethodBase.GetCurrentMethod(), activationRequestUri);
        }
    }
}
