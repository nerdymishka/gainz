
using NerdyMishka.Reflection;
using NerdyMishka.Reflection.Extensions;

internal static class Extensions
{
    public static bool IsListLike(this IType typeInfo)
    {
        return typeInfo.IsArray() || typeInfo.IsIListOfT() || typeInfo.IsICollectionOfT() || typeInfo.IsIList() || typeInfo.IsICollection();
    }

    public static bool IsDictionaryLike(this IType typeInfo)
    {
        return typeInfo.IsIDictionaryOfKv() || typeInfo.IsIDictionary();
    }
}