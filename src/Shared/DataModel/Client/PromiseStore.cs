using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.DataModel
{
    internal class PromiseStore
    {
        private readonly Dictionary<string, TaskCompletionSource<object>> Promises = new();

        public string Create()
        {
            string id = Guid.NewGuid().ToString("n");
            Promises.Add(id, new());
            return id;
        }

        public void Complete(string id, object value)
        {
            if (Promises.TryGetValue(id, out var source))
            {
                source.SetResult(value);
            }
        }

        public void Fail(string id, ErrorInfo error)
        {
            if (Promises.TryGetValue(id, out var source))
            {
                source.SetException(new Exception(error.Message));
            }
        }

        public async Task<T> GetTask<T>(string id)
        {
            if (Promises.TryGetValue(id, out var source))
            {
                object value = await source.Task;
                
                if (value is T tval)
                    return tval;
                else if (value is JToken token)
                    return token.ToObject<T>();
                return JToken.FromObject(value).ToObject<T>();
            }
            throw new KeyNotFoundException();
        }
    }
}
