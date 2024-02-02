using System;
using System.Drawing;

namespace Raid.Toolkit.Extensibility;

public interface IWindowAdapter
{
	object Owner { get; }
	string TypeName { get; }
	Point Location { get; set; }
	Size Size { get; set; }
	void Show();
	event EventHandler<WindowAdapterCloseEventArgs> Closing;
	event EventHandler<WindowAdapterEventArgs> Shown;
	event EventHandler<WindowAdapterSizeChangedEventArgs> Resized;
}
public interface IWindowAdapter<out T> : IWindowAdapter where T : class
{
	T GetOwner();
}
public class WindowAdapterEventArgs : EventArgs
{
	public string OwnerType { get; }
	public WindowAdapterEventArgs(string ownerType)
	{
		OwnerType = ownerType;
	}
}
public class WindowAdapterCloseEventArgs : WindowAdapterEventArgs
{
	public bool IsUserClose { get; }
	public WindowAdapterCloseEventArgs(string ownerType, bool isUserClose)
		: base(ownerType)
	{
		IsUserClose = isUserClose;
	}
}
public class WindowAdapterSizeChangedEventArgs : WindowAdapterEventArgs
{
	public Windows.Foundation.Rect Bounds { get; }
	public WindowAdapterSizeChangedEventArgs(string ownerType, Windows.Foundation.Rect bounds)
		: base(ownerType)
	{
		Bounds = bounds;
	}
}
