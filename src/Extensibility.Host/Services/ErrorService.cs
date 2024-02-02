using System;
using System.Collections.Generic;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class ErrorEventArgs : EventArgs
    {
        public ServiceErrorCategory Category { get; }
        public ServiceError ErrorCode { get; }
        public string TargetDescription { get; }
        public string ErrorMessage { get; set; }
        public string HelpMessage { get; set; }
        public object Target { get; }
        public ulong Count { get; set; }
        public ulong Threshold { get; set; }
        public bool Notified { get; set; }

        public string Key => $"{Category}:{TargetDescription}";

        public ErrorEventArgs(ServiceError errorCode, ServiceErrorCategory category, string targetDescription, object target)
        {
            ErrorCode = errorCode;
            Category = category;
            TargetDescription = targetDescription;
            Target = target;
			ErrorMessage = string.Empty;
			HelpMessage = string.Empty;
        }
    }

    public class ErrorService
    {
        public readonly Dictionary<string, ErrorEventArgs> CurrentErrors = new();
        public event EventHandler<ErrorEventArgs>? OnError;
        public event EventHandler<ErrorEventArgs>? OnErrorAdded;
        public event EventHandler<ErrorEventArgs>? OnErrorCleared;

        public TrackedOperation TrackOperation(ServiceErrorCategory category, string targetDescription, object target)
        {
            return new TrackedOperation(this, category, targetDescription, target);
        }

        public void EmitError(ErrorEventArgs args)
        {
            _ = CurrentErrors.TryAdd(args.Key, args);
            var storedArgs = CurrentErrors[args.Key];
            ++storedArgs.Count;
            if (storedArgs.Count >= storedArgs.Threshold && !storedArgs.Notified)
            {
                storedArgs.Notified = true;
                OnErrorAdded?.Raise(this, storedArgs);
            }

            OnError?.Raise(this, args);
        }

        public void ClearError(ServiceErrorCategory category, string targetDescription)
        {
            if (CurrentErrors.Remove($"{category}:{targetDescription}", out var args))
                OnErrorCleared?.Raise(this, args);
        }
    }

    public class TrackedOperation : IDisposable
    {
        private bool disposedValue;

        private readonly ErrorService ErrorService;
        private bool Failed;
        public ServiceErrorCategory Category { get; }
        public string TargetDescription { get; }
        public object Target { get; }
        public TrackedOperation(ErrorService errorService, ServiceErrorCategory category, string targetDescription, object target)
        {
            Category = category;
            TargetDescription = targetDescription;
            Target = target;
            ErrorService = errorService;
        }

        public void Fail(ServiceError errorCode, ulong threshold = 0)
        {
            if (Failed)
                return;
            Failed = true;
            ErrorService.EmitError(new(errorCode, Category, TargetDescription, Target) { Threshold = threshold });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!Failed)
                    {
                        ErrorService.ClearError(Category, TargetDescription);
                    }
                }
                disposedValue = true;
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
