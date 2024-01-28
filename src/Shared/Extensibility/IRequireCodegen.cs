using System;

namespace Raid.Toolkit.Extensibility;

[Obsolete("not used")]
public interface IRequireCodegen
{
	CodegenTypeFilter TypeFilter { get; }
}

