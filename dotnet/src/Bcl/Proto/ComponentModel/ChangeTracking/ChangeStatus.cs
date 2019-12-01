

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    public enum ChangeStatus : byte
    {
        None = 0,
        Detached = 1,
        New = 2,
        Modified = 4,
        Deleted = 8
    }
}