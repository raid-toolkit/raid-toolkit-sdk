using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.DataModel
{
    public class CallMethodMessage : AsyncMessage
    {
        [JsonProperty("methodName")]
        public string MethodName;

        [JsonProperty("args")]
        public JArray Parameters;
    }
}
