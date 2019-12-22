
using NerdyMishka.Reflection;
using NerdyMishka.Reflection.Extensions;

internal static class Extensions
{
    public static bool IsListLke(this IType typeInfo)
    {
        return typeInfo.IsArray() || typeInfo.IsICollectionOfT() || typeInfo.IsIListOfT() || typeInfo.IsICollection() || typeInfo.IsIList();
    }

    public static bool IsDictionaryLike(this IType typeInfo)
    {
        return typeInfo.IsIDictionaryOfKv() || typeInfo.IsIDictionary();
    }
}