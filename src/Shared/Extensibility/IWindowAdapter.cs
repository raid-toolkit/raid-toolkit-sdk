using System;
using System.Drawing;

namespace Raid.Toolkit.Extensibility
{
    public interface IWindowAdapter
    {
        public object Owner { get; }
        public string TypeName { get; }
        public Point Location { get; set; }
        public Size Size { get; set; }
        public void Show();
        public event EventHandler<WindowAdapterCloseEventArgs> Closing;
        public event EventHandler<WindowAdapterEventArgs> Shown;
        public event EventHandler<WindowAdapterSizeChangedEventArgs> Resized;
    }
    public interface IWindowAdapter<T> : IWindowAdapter where T : class
    {
        public T GetOwner();
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
}
