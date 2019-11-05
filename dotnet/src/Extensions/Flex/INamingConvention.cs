namespace NerdyMishka.Flex
{
    public interface INamingConvention
    {
        string Name { get; }
        string Provider { get; set; }
        string Transform(string value);
    }
}