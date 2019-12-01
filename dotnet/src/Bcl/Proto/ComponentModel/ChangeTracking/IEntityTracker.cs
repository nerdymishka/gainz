namespace NerdyMishka.ComponentModel.ChangeTracking
{
    public interface IEntityTracker
    {
        ChangeStatus ChangeStatus { get; }

        void SetChangeStatus(ChangeStatus status);

        bool HasChangeStatus(ChangeStatus status);
    }
    
}