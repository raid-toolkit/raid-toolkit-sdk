using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.UI.Xaml;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IWindowAdapter
    {
        public T GetOwner<T>() where T : class;
        public string TypeName { get; }
        public Point Location { get; set; }
        public Size Size { get; set; }
        public void Show();
        public event EventHandler<WindowAdapterCloseEventArgs> Closing;
        public event EventHandler<WindowAdapterEventArgs> Shown;
        public event EventHandler<WindowAdapterSizeChangedEventArgs> Resized;

        public static IWindowAdapter Create(object target)
        {
            if (target is IWindowAdapter adapter)
                return adapter;

            if (target is Form form)
                return new FormAdapter(form);

            if (target is Window wnd)
                return new WinUIAdapter(wnd);

            throw new NotSupportedException();
        }
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
    public class FormAdapter : IWindowAdapter
    {
        private readonly Form Owner;
        public string TypeName => Owner.GetType().Name;
        public T GetOwner<T>() where T : class => Owner is T value ? value : throw new InvalidCastException();
        public event EventHandler<WindowAdapterCloseEventArgs>? Closing;
        public event EventHandler<WindowAdapterEventArgs>? Shown;
        public event EventHandler<WindowAdapterSizeChangedEventArgs>? Resized;

        public FormAdapter(Form form)
        {
            Owner = form;
            Owner.Shown += (_, e) =>
            {
                Shown?.Invoke(this, new(TypeName));
            };
            Owner.FormClosing += (_, e) =>
            {
                bool isUserClose = e.CloseReason is CloseReason.UserClosing
                    or CloseReason.FormOwnerClosing;

                Closing?.Invoke(this, new(TypeName, isUserClose));
            };
            Owner.ResizeEnd += (_, e) =>
            {
                Resized?.Invoke(this, new(TypeName, Owner.Bounds));
            };
        }

        public void Show()
        {
            Owner.Show();
        }

        public Point Location
        {
            get => Owner.Location;
            set
            {
                Owner.StartPosition = FormStartPosition.Manual;
                Owner.Location = value;
            }
        }
        public Size Size
        {
            get => Owner.Size;
            set
            {
                Owner.StartPosition = FormStartPosition.Manual;
                Owner.Size = value;
            }
        }
    }
    public class WinUIAdapter : IWindowAdapter
    {
        private readonly Window Owner;
        public string TypeName => Owner.GetType().Name;
        public T GetOwner<T>() where T : class => Owner is T value ? value : throw new InvalidCastException();
        public event EventHandler<WindowAdapterCloseEventArgs>? Closing;
        public event EventHandler<WindowAdapterEventArgs>? Shown;
        public event EventHandler<WindowAdapterSizeChangedEventArgs>? Resized;

        public WinUIAdapter(Window wnd)
        {
            Owner = wnd;
            Owner.VisibilityChanged += (_, e) =>
            {
                if (e.Visible)
                    Shown?.Invoke(this, new(TypeName));
            };
            Owner.Closed += (_, e) =>
            {
                Closing?.Invoke(this, new(TypeName, true));
            };
            Owner.SizeChanged += (_, e) =>
            {
                Resized?.Invoke(this, new(TypeName, new(
                        (int)Owner.Bounds.X, (int)Owner.Bounds.Y,
                        (int)Owner.Bounds.Width, (int)Owner.Bounds.Height)
                    ));
            };
        }

        public void Show()
        {
            Owner.Activate();
        }

        public Point Location
        {
            get
            {
                return new((int)Owner.Bounds.X, (int)Owner.Bounds.Y);
            }
            set { } // TODO
        }
        public Size Size
        {
            get
            {
                return new((int)Owner.Bounds.Width, (int)Owner.Bounds.Height);
            }
            set { } // TODO
        }
    }
}
