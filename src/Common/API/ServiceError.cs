namespace Raid.Toolkit.Common.API;

public enum ApiError
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
	MethodCalledBeforeInitialization,
	StaticDataReadError,
}
