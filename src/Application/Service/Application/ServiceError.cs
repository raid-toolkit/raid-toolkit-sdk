using Il2CppToolkit.Common.Errors;

namespace Raid.Service
{
    [ErrorCategory("Service Error", "SE")]
    public enum ServiceError
    {
        MessageHandlerFailure = 5000,
        UnknownMessageScope,
        MessageProcessingFailure,
        ApiProxyException,
        AccountUpdateFailed,
        AccountNotReady,
        ObjectReadError,
        MissingUpdateAsset,
        UnhandledException,
        ThreadException,
        AccountReadError,
        ProcessAccessDenied,
    }
}
