using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.Common.API.Messages;

public class AsyncMessage
{
	[JsonProperty("promiseId")]
	public string PromiseId = string.Empty;

	public AsyncMessage()
	{
		PromiseId = string.Empty;
	}

	public AsyncMessage(string promiseId)
	{
		PromiseId = promiseId;
	}

	public async Task<JToken> Resolve(object? result)
	{
		object? value = null;
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

			Type taskType = task.GetType();
			if (taskType.IsGenericType && taskType.GenericTypeArguments[0].Name != "VoidTaskResult")
			{
				value = ((dynamic)task).Result;
			}
		}
		else
		{
			value = result;
		}

		return JObject.FromObject(new PromiseSucceededMessage(PromiseId, value));
	}

	public JToken Reject(Exception ex)
	{
		return JObject.FromObject(new PromiseFailedMessage(PromiseId, new ErrorInfo(ex)));
	}
}
