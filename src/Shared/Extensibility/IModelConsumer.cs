using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Raid.Toolkit.Extensibility;

[Obsolete("not used")]
public interface IModelConsumer
{
	IEnumerable<Regex> TypeNameMatchers { get; }
}
