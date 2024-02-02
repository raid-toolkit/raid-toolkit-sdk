using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host
{
	public abstract class PollingBackgroundService : IHostedService, IDisposable
	{
		protected readonly ILogger Logger;
		private static readonly TimeSpan DefaultPollInterval = new(0, 0, 10);
		private protected virtual TimeSpan PollInterval => DefaultPollInterval;

		private Task? CurrentTask;
		private CancellationTokenSource? CurrentTaskCanellationTokenSource;
		private bool IsDisposed;

		public PollingBackgroundService(ILogger logger)
		{
			Logger = logger;
		}

		protected abstract Task ExecuteOnceAsync(CancellationToken token);

		public virtual Task StartAsync(CancellationToken cancellationToken)
		{
			CurrentTaskCanellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			CurrentTask = Task.Run(ExecuteAsync, cancellationToken);
			return Task.CompletedTask;
		}

		public virtual async Task StopAsync(CancellationToken cancellationToken)
		{
			if (CurrentTask == null)
				return;

			try
			{
				CurrentTaskCanellationTokenSource?.Cancel();
			}
			finally
			{
				try
				{
					await Task.WhenAny(CurrentTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Failed to cancel service");
				}
			}
		}

		private async Task ExecuteAsync()
		{
			if (CurrentTaskCanellationTokenSource == null)
				throw new ApplicationException("Cannot run background service without a cancellation source");

			await ExecuteOnceAsync(CurrentTaskCanellationTokenSource.Token);
			CurrentTask = Task.Delay((int)PollInterval.TotalMilliseconds, CurrentTaskCanellationTokenSource.Token).ContinueWith(_ => ExecuteAsync());
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					CurrentTaskCanellationTokenSource?.Cancel();
				}

				CurrentTaskCanellationTokenSource = null;
				IsDisposed = true;
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
