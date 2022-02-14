namespace Raid.Toolkit.Extensibility.Services
{
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
