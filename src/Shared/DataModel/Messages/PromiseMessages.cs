using System;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class PromiseMessage : AsyncMessage
    {
        [JsonProperty("success")]
        public bool Success;
    }

    public class PromiseSuccededMessage : PromiseMessage
    {
        [JsonProperty("value")]
        public object Value;
    }

    public class PromiseFailedMessage : PromiseMessage
    {
        [JsonProperty("error")]
        public ErrorInfo ErrorInfo;
    }

    public class ErrorInfo
    {
        [JsonProperty("message")]
        public string Message;

        public ErrorInfo() { }
        public ErrorInfo(Exception ex)
        {
            Message = ex?.Message;
        }
    }
}
