using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.DataModel
{
    public class AsyncMessage
    {
        [JsonProperty("promiseId")]
        public string PromiseId;

        public async Task<JToken> Resolve(object result)
        {
            object value = null;
            if (result is Task task)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    return Reject(ex);
                }

                if (task.GetType().IsGenericType)
                {
                    value = ((dynamic)task).Result;
                }
            }
            else
            {
                value = result;
            }

            return JObject.FromObject(new PromiseSuccededMessage()
            {
                PromiseId = PromiseId,
                Success = true,
                Value = value
            });
        }

        public JToken Reject(Exception ex)
        {
            return JObject.FromObject(new PromiseFailedMessage()
            {
                PromiseId = PromiseId,
                Success = false,
                ErrorInfo = new ErrorInfo(ex)
            });
        }
    }
}
