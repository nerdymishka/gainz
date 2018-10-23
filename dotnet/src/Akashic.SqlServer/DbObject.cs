namespace NerdyMishka.Data
{


    public class DbObject
    {


        public virtual string Name { get; internal protected set; }

        public virtual bool IsDirty { get; internal protected set; }

        public virtual bool IsDeleted { get; internal protected set; }

        public virtual bool IsCreated { get; internal protected set; } 

        
    }
}