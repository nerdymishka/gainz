using System.ComponentModel;

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    public interface IChangeModel : INotifyPropertyChanged, IDataErrorInfo
    {
        ChangeStatus ChangeStatus { get; }

        void SetChangeStatus(ChangeStatus status);

        bool HasChangeStatus(ChangeStatus status);
    }
}