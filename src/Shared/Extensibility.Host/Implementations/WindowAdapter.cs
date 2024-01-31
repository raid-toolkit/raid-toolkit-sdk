using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.UI.Xaml;

namespace Raid.Toolkit.Extensibility.Host;

public class FormAdapter<T> : IWindowAdapter<T> where T : class
{
	object IWindowAdapter.Owner => Form;
	private readonly Form Form;
	public string TypeName => Form.GetType().Name;
	public T GetOwner() => Form is T value ? value : throw new InvalidCastException();
	public event EventHandler<WindowAdapterCloseEventArgs>? Closing;
	public event EventHandler<WindowAdapterEventArgs>? Shown;
	public event EventHandler<WindowAdapterSizeChangedEventArgs>? Resized;

	public FormAdapter(Form form)
	{
		Form = form;
		Form.Shown += (_, e) =>
		{
			Shown?.Invoke(this, new(TypeName));
		};
		Form.FormClosing += (_, e) =>
		{
			bool isUserClose = e.CloseReason is CloseReason.UserClosing
				or CloseReason.FormOwnerClosing;

			Closing?.Invoke(this, new(TypeName, isUserClose));
		};
		Form.ResizeEnd += (_, e) =>
		{
			Resized?.Invoke(this, new(TypeName, new(
					Form.Bounds.X, Form.Bounds.Y,
					Form.Bounds.Width, Form.Bounds.Height)
				));
		};
	}

	public void Show()
	{
		Form.Show();
	}

	public Point Location
	{
		get => Form.Location;
		set
		{
			Form.StartPosition = FormStartPosition.Manual;
			Form.Location = value;
		}
	}
	public Size Size
	{
		get => Form.Size;
		set
		{
			Form.StartPosition = FormStartPosition.Manual;
			Form.Size = value;
		}
	}
}
public class WinUIAdapter<T> : IWindowAdapter<T> where T : class
{
	object IWindowAdapter.Owner => Window;
	private readonly Window Window;
	public string TypeName => Window.GetType().Name;
	public T GetOwner() => Window is T value ? value : throw new InvalidCastException();
	public event EventHandler<WindowAdapterCloseEventArgs>? Closing;
	public event EventHandler<WindowAdapterEventArgs>? Shown;
	public event EventHandler<WindowAdapterSizeChangedEventArgs>? Resized;

	public WinUIAdapter(Window wnd)
	{
		Window = wnd;
		Window.VisibilityChanged += (_, e) =>
		{
			if (e.Visible)
				Shown?.Invoke(this, new(TypeName));
		};
		Window.Closed += (_, e) =>
		{
			Closing?.Invoke(this, new(TypeName, true));
		};
		Window.SizeChanged += (_, e) =>
		{
			Resized?.Invoke(this, new(TypeName, Window.Bounds));
		};
	}

	public void Show()
	{
		Window.Activate();
	}

	public Point Location
	{
		get
		{
			return new((int)Window.Bounds.X, (int)Window.Bounds.Y);
		}
		set { } // TODO
	}
	public Size Size
	{
		get
		{
			return new((int)Window.Bounds.Width, (int)Window.Bounds.Height);
		}
		set { } // TODO
	}
}
