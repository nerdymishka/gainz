

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    public enum ChangeStatus : byte
    {
        None = 0,
        Added = 1,
        Changed = 2,
        Deleted = 4
    }
}