using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Raid.Toolkit.Extensibility
{
	public interface IRequireCodegen
	{
		public IEnumerable<Regex> TypePatterns { get; }
	}
}
