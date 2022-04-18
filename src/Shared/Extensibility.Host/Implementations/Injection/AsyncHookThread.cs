using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Raid.Toolkit.Injection
{
	public class AsyncHookThread : IDisposable
	{
		private readonly Thread m_thread;
		private readonly AutoResetEvent m_signal;
		private readonly ConcurrentQueue<Action> m_actionQueue = new();
		private volatile bool m_disposing;
		private volatile bool m_disposed;

		private static AsyncHookThread s_current;
		public static AsyncHookThread Current
		{
			get
			{
				if (s_current == null)
					s_current = new();
				return s_current;
			}
		}

		public static void DisposeCurrent()
		{
			s_current?.Dispose();
			s_current = null;
		}

		public AsyncHookThread()
		{
			m_thread = new Thread(ThreadStart);
			m_signal = new(false);
			m_thread.Start();
		}

		public void Post(Action callback)
		{
			m_actionQueue.Enqueue(callback);
			m_signal.Set();
		}

		private void ThreadStart()
		{
			m_signal.WaitOne();
			while (!m_disposing)
			{
				while (m_actionQueue.TryDequeue(out Action callback))
				{
					callback();
				}
				m_signal.WaitOne();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (disposing)
				{
					m_actionQueue.Clear();
					m_disposing = true;
					m_signal.Set();
					m_thread.Join();
					m_signal.Dispose();
				}

				m_disposed = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
