using Il2CppToolkit.Common.Errors;

namespace Raid.Service
{
    [ErrorCategory("Service Error", "SE")]
    public enum ServiceError
    {
        MessageProcessingFailure = 5000,
    }
}