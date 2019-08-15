namespace Nebukam.Beacon
{
    [System.Flags]
    public enum IntentionState
    {
        IDLE = 1,
        SEEK = 2,
        REACHED = 3,
        ANY = IDLE | SEEK | REACHED
    }
}
