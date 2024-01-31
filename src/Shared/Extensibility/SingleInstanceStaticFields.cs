using System;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility;

[Obsolete("not used")]
[Size(16)]
public struct SingleInstanceStaticFields<T>
{
	[Offset(8)]
#pragma warning disable 649
	public T Instance;
#pragma warning restore 649
}
