namespace Nebukam.Beacon
{
    [System.Flags]
    public enum UpdateOrder
    {
        Before = 1,
        After = 2,
        BeforeAndAfter = Before | After
    }
}
