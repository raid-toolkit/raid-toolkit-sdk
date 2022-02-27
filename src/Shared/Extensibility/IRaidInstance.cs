using Il2CppToolkit.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility
{
	public interface IRaidInstance
	{
		public string Id { get; }
		public Il2CsRuntimeContext Runtime { get; }
	}
}
