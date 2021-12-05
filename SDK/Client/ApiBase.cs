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

        protected Task<U> CallMethod<U>(MethodBase method, params object[] args)
        {
            return !Api.Members.TryGetValue(method.Name, out var def)
                ? Task.FromException<U>(new MissingMethodException(typeof(T).Name, method.Name))
                : Client.Call<U>(def.Scope, def.Attribute.Name, args);
        }

        protected void Subscribe(EventInfo eventInfo)
        {
            if (!Api.Members.TryGetValue(eventInfo.Name, out var def))
            {
                throw new MissingMethodException(typeof(T).Name, eventInfo.Name);
            }
            Client.Subscribe(def.Scope, def.Attribute.Name);
        }
    }
}