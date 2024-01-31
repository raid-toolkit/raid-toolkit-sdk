using System;
using System.Runtime.InteropServices;

using Microsoft.UI;

namespace Raid.Toolkit.UI.WinUI
{
	public class EmbeddedIconId : IDisposable
	{
		private IconId _iconId = new();
		private IntPtr _iconHandle = IntPtr.Zero;
		private bool _isDisposed;

		public IconId Value => _iconId;

		public EmbeddedIconId(long iconIndex)
		{
			var index = (IntPtr)iconIndex; // 0 = first icon in resources
			_iconHandle = ExtractAssociatedIcon(IntPtr.Zero, RTKApplication.ExecutablePath, ref index);
			if (_iconHandle != IntPtr.Zero)
			{
				_iconId = Win32Interop.GetIconIdFromIcon(_iconHandle);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				if (_iconHandle != IntPtr.Zero)
				{
					DestroyIcon(_iconHandle);
					_iconHandle = IntPtr.Zero;
				}
				_iconId = new IconId();

				_isDisposed = true;
			}
		}

		~EmbeddedIconId()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		[DllImport("user32.dll", SetLastError = true)]
		static extern int DestroyIcon(IntPtr hIcon);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string iconPath, ref IntPtr index);
	}
}
