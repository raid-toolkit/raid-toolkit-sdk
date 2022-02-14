using System.Reflection;
using System.Threading.Tasks;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class ApiBase<T>
    {
        private readonly RaidToolkitClient Client;
        private readonly PublicApiInfo<T> Api = new();
        internal ApiBase(RaidToolkitClient client)
        {
            Client = client;
        }

        protected Task<U> CallMethod<U>(MethodBase method, params object[] args)
        {
            var def = Api.GetMember<MethodInfo>(method.Name);
            return Client.Call<U>(def.Scope, def.Attribute.Name, args);
        }

        protected void Subscribe(EventInfo eventInfo)
        {
            var def = Api.GetMember<EventInfo>(eventInfo.Name);
            Client.Subscribe(def.Scope, def.Attribute.Name);
        }
    }
}
