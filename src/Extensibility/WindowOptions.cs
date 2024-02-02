using System;

namespace Raid.Toolkit.Extensibility;

public class WindowOptions
{
	public Func<object>? Create { get; set; }
	public bool RememberPosition { get; set; }
	public bool RememberVisibility { get; set; }
}
