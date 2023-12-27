using System;

namespace Raid.Toolkit.Common;

public class V3NotImpl : Exception
{
}

[AttributeUsage(AttributeTargets.All)]
public class DeprecatedInV3 : Attribute
{ }
