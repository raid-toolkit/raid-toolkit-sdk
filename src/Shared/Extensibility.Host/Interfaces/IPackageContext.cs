using System;

namespace Raid.Toolkit.Extensibility;

[Obsolete("not used")]
public interface IPackageContext
{
	PackageDescriptor Descriptor { get; }
}
