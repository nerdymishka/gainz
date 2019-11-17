using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class FlexMethodInfo
    {
        private MethodInfo methodInfo;

        public string Name => this.methodInfo.Name;

        private FlexTypeInfo returnType;

        public FlexTypeInfo ReturnType => this.returnType;


        public FlexMethodInfo(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            this.returnType = ReflectionCache.GetOrAdd(this.methodInfo.ReturnType);
        }
        
    }
}