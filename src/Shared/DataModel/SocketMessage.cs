using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Raid.Toolkit.DataModel
{
    internal class SocketMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = existingValue ?? Activator.CreateInstance(objectType);
            var fields = objectType.GetFields().Select(field => new { Attribute = field.GetCustomAttribute<JsonPropertyAttribute>(), Field = field }).OrderBy(entry => entry.Attribute.Order);
            var array = JArray.Load(reader);
            foreach (var entry in fields)
            {
                if (array.Count <= entry.Attribute.Order) continue;
                entry.Field.SetValue(value, array[entry.Attribute.Order].ToObject(entry.Field.FieldType, serializer));
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var properties = value.GetType().GetFields()
                .Select(field => new { Attribute = field.GetCustomAttribute<JsonPropertyAttribute>(), Field = field })
                .OrderBy(entry => entry.Attribute.Order)
                .ToList();
            object[] array = new object[properties.Count];
            foreach (var entry in properties)
            {
                array[entry.Attribute.Order] = JToken.FromObject(entry.Field.GetValue(value), serializer);
            }

            JArray jarray = new(array);
            jarray.WriteTo(writer);
        }
    }

    [JsonConverter(typeof(SocketMessageConverter))]
    public class SocketMessage
    {
        [JsonProperty(Order = 0)]
        public string Scope;

        [JsonProperty(Order = 1)]
        public string Channel;

        [JsonProperty(Order = 2)]
        public JToken Message;
    }
}
