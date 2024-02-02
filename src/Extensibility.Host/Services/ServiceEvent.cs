namespace Raid.Toolkit.Extensibility.Host.Services
{
    public enum ServiceEvent : int
    {
        HandleMessage = 0,
        UserPermissionRequest,
        UserPermissionCached,
        UserPermissionAccept,
        UserPermissionReject,
        MissingSkill,
        DataUpdated,
        ReadObject,
    }
}
