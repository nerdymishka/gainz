

#if !NETSTANDARD2_1

namespace System.Diagnostics.CodeAnalysis
{

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class AllowNullAttribute : System.Attribute
    {
      
        
        // This is a positional argument
        public AllowNullAttribute()
        {
            
        }
        

    }
}

#endif 