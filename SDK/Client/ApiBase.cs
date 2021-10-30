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
            if (!Api.Methods.TryGetValue(method.Name, out var def))
            {
                return Task.FromException<U>(new MissingMethodException(typeof(T).Name, method.Name));
            }
            return Client.Call<U>(def.Scope, def.Attribute.Name, args);
        }
    }
}