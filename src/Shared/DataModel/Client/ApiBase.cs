using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel
{
    public class ApiBase<T>
    {
        private readonly RaidToolkitClientBase Client;
        private readonly PublicApiInfo<T> Api = new();
        protected ApiBase(RaidToolkitClientBase client)
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
