using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Linq;

public static class JsonExtensions
{
	public static T ToObjectOrThrow<T>(this JToken obj)
	{
		T? result = obj.ToObject<T>();
		if (result == null)
			throw new NullReferenceException(nameof(result));

		return result;
	}
}
