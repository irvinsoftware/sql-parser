namespace Irvin.SqlParser.Metadata
{
    public enum DatabaseState
    {
        Online,
        Restoring,
        Recovering,
        RecoveryPending,
        Suspect,
        Emergency,
        Offline,
        Copying,
        OfflineSecondary
    }
}