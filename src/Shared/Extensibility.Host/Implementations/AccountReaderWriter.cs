using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Raid.Toolkit.Extensibility.Host;

public class SerializedAccountRecord
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public SerializedAccountRecord()
	{
		DataContext = string.Empty;
		Key = string.Empty;
		Value = new();
	}
	public SerializedAccountRecord(ExtensionDataContext context, string key, object value)
	{
		DataContext = context.ToString();
		Key = key;
		Value = JObject.FromObject(value);
	}

	[JsonProperty("d")]
	public string DataContext { get; set; }
	[JsonProperty("k")]
	public string Key { get; set; }
	[JsonProperty("v")]
	public JObject Value { get; set; }
}

public class SerializedAccountData
{
	public SerializedAccountData() { }
	public SerializedAccountData(AccountBase account) => Info = account;

	[JsonProperty("a")]
	public AccountBase? Info { get; set; }

	[JsonProperty("r")]
	public List<SerializedAccountRecord> Records { get; } = new();

	public void AddData<T>(ExtensionDataContext context, string key, T value) where T : class
	{
		Records.Add(new(context, key, value));
	}
	public bool TryGetData<T>(ExtensionDataContext context, string key, [NotNullWhen(true)] out T? value) where T : class
	{
		SerializedAccountRecord? record = Records.FirstOrDefault(entry => entry.DataContext == context.ToString() && entry.Key == key);
		value = record?.Value.ToObject<T>();
		if (value != null)
		{
			return true;
		}
		return false;
	}

	public IAccountReaderWriter CreateReaderWriter(ExtensionDataContext context)
	{
		return new AccountReaderWriter(this, context);
	}
}

public class AccountReaderWriter : IAccountReaderWriter
{
	private readonly SerializedAccountData Data;
	private readonly ExtensionDataContext Context;
	public AccountReaderWriter(SerializedAccountData data, ExtensionDataContext context)
	{
		Data = data;
		Context = context;
	}

	public bool TryRead<T>(string key, [NotNullWhen(true)] out T? value) where T : class
	{
		return Data.TryGetData(Context, key, out value);
	}

	public void Write<T>(string key, T value) where T : class
	{
		Data.AddData(Context, key, value);
	}
}
