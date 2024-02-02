using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Raid.Toolkit.Common.API.Messages;

public class SocketMessageConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return true;
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		var value = existingValue ?? Activator.CreateInstance(objectType);
		var fields = objectType.GetFields().Select(field => new { Attribute = field.GetCustomAttribute<JsonPropertyAttribute>()!, Field = field }).OrderBy(entry => entry.Attribute.Order);
		var array = JArray.Load(reader);
		foreach (var entry in fields)
		{
			if (array.Count <= entry.Attribute.Order) continue;
			entry.Field.SetValue(value, array[entry.Attribute.Order].ToObject(entry.Field.FieldType, serializer));
		}
		return value;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		var properties = value?.GetType().GetFields()
			.Select(field => new { Attribute = field.GetCustomAttribute<JsonPropertyAttribute>()!, Field = field })
			.OrderBy(entry => entry.Attribute.Order)
			.ToList() ?? new();
		object[] array = new object[properties.Count];
		foreach (var entry in properties)
		{
			array[entry.Attribute.Order] = JToken.FromObject(entry.Field.GetValue(value)!, serializer);
		}

		JArray jarray = new(array);
		jarray.WriteTo(writer);
	}
}

[JsonConverter(typeof(SocketMessageConverter))]
public class SocketMessage
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public SocketMessage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	{
		Scope = string.Empty;
		Channel = string.Empty;
	}

	public SocketMessage(string scope, string channel, JToken message)
	{
		Scope = scope;
		Channel = channel;
		Message = message;
	}

	[JsonProperty(Order = 0)]
	public string Scope;

	[JsonProperty(Order = 1)]
	public string Channel;

	[JsonProperty(Order = 2)]
	public JToken Message;
}
