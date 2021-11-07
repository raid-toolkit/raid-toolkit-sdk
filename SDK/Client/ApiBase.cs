using System;
using System.Reflection;
using System.Threading.Tasks;
using Raid.DataModel;

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

        protected void Subscribe(EventInfo eventInfo)
        {
            if (!Api.Members.TryGetValue(eventInfo.Name, out var def))
            {
                throw new MissingMethodException(typeof(T).Name, eventInfo.Name);
            }
            Client.Subscribe(def.Scope, def.Attribute.Name);
        }

        protected Task<U> CallMethod<U>(MethodBase method, params object[] args)
        {
            if (!Api.Members.TryGetValue(method.Name, out var def))
            {
                return Task.FromException<U>(new MissingMethodException(typeof(T).Name, method.Name));
            }
            return Client.Call<U>(def.Scope, def.Attribute.Name, args);
        }
    }
}