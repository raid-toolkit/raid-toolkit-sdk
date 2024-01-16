using System;
using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility;

public class WindowOptions
{
	[Obsolete("use Create")]
	public Func<Form>? CreateInstance { get; set; }
	public Func<object>? Create { get; set; }
	public bool RememberPosition { get; set; }
	public bool RememberVisibility { get; set; }
}
