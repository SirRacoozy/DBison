namespace DBison.Core.Entities;
public enum eDataBaseState
{
    ONLINE = 0,
    RESTORING = 1,
    RECOVERING = 2,
    RECOVERY_PENDING = 3,
    SUSPECT = 4,
    EMERGENCY = 5,
    OFFLINE = 6,
    COPYING = 7,
    OFFLINE_SECONDARY = 10,
}