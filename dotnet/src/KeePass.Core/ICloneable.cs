namespace NerdyMishka.KeePass
{
    internal interface ICloneable<T>
    {
        T Clone(IKeePassPackage owner);
    }
}