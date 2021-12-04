using System;
using System.Collections.Generic;

namespace Raid.Service
{
    public class ErrorEventArgs : EventArgs
    {
        public ServiceErrorCategory Category { get; }
        public ServiceError ErrorCode { get; }
        public string TargetDescription { get; }
        public string ErrorMessage { get; set; }
        public string HelpMessage { get; set; }
        public object Target { get; }
        public string Key => $"{Category}:{ErrorCode}:{TargetDescription}";
        public ErrorEventArgs(ServiceError errorCode, ServiceErrorCategory category, string targetDescription, object target)
        {
            ErrorCode = errorCode;
            Category = category;
            TargetDescription = targetDescription;
            Target = target;
        }
    }
    public class ErrorService : IdleBackgroundService
    {
        public readonly Dictionary<string, ErrorEventArgs> CurrentErrors = new();
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<ErrorEventArgs> OnErrorAdded;
        public event EventHandler<ErrorEventArgs> OnErrorCleared;

        public void EmitError(ErrorEventArgs args)
        {
            if (CurrentErrors.TryAdd(args.Key, args))
                OnErrorAdded?.Invoke(this, args);

            OnError?.Invoke(this, args);
        }

        public void ClearError(ErrorEventArgs args)
        {
            if (CurrentErrors.Remove(args.Key))
                OnError?.Invoke(this, args);
        }
    }
}
