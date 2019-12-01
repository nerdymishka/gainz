
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    public class ChangeModel : IChangeModel, IChildModel, IParentModel
    {
        private IList<IChildModel> children;

        private IParentModel parent;

        private IDictionary<string, object> properties;
        private PropertyChangedEventHandler PropertyChanged;
        private PropertyChangingEventHandler PropertyChanging;
        protected IDictionary<string, string> errorInfo;
        private ChangeStatus status;

        string IDataErrorInfo.this[string columnName] {
            get { 
                if(this.errorInfo == null)
                    return null;

                if(this.errorInfo.TryGetValue(columnName, out string error))
                    return error;

                return null;
            }
        }

        string IDataErrorInfo.Error => this.CollectErrors(this.errorInfo);

        ChangeStatus IChangeModel.ChangeStatus => this.status;

        //IDictionary<string, object> IModifiedProperties.ModifiedProperties => this.properties;

        IEnumerable<IChildModel> IParentModel.Children => this.children;

        IParentModel IChildModel.Parent { 
            get => this.parent;
            set => this.parent= value; 
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => this.PropertyChanged += value;
            remove => this.PropertyChanged -= value;
        }

        protected virtual T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if(this.properties.TryGetValue(propertyName, out object value))
                return (T)value;

            return default(T);
        }


        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if(EqualityComparer<T>.Default.Equals(field, value))
                return;

            var interned = String.IsInterned(propertyName);
            if(interned == null)
                interned = String.Intern(propertyName);

            this.properties = properties ?? new Dictionary<string, object>();
            this.properties[interned] = value;
            field = value;
            this.SetChangeStatus(ChangeStatus.Modified);
            this.NotifyPropertyChanged(propertyName);
        }

        protected virtual string CollectErrors(IDictionary<string, string> errors)
        {
            if(errors == null)
                return null;

            if(errors.Count == 0)
                return null;

            return $"{this.GetType().Name} has {errors.Count} errors";
        }

        protected virtual void SetChangeStatus(ChangeStatus status)
        {
            if(this.status.HasFlag(status))
                return;

            switch(this.status)
            {
                case ChangeStatus.None:
                case ChangeStatus.Detached:
                    this.status = status;
                    return;
            }

            switch(status)
            {
                case ChangeStatus.Detached:
                case ChangeStatus.None:
                    this.status = status;
                    return;

                default:
                    this.status= this.status | status;
                    break;
            }
        }

        protected virtual void NotifyPropertyChanging(string name)
            => this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));

        protected virtual void NotifyPropertyChanged(string name)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        void IChangeModel.SetChangeStatus(ChangeStatus status)
        {
            this.SetChangeStatus(status);
        }

        bool IChangeModel.HasChangeStatus(ChangeStatus status)
        {
            return this.status.HasFlag(status);
        }

        void IParentModel.Attach(IChildModel model)
        {
            model.Parent = this;
            this.children.Add(model);
        }

        void IParentModel.Detach(IChildModel model)
        {
            model.Parent = null;
            this.children.Remove(model);
        }
    }
}