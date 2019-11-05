namespace NerdyMishka.Search.Documents
{
    public interface IField
    {
        string Name { get; }

        string Value { get; set; }

        StorageStrategy Storage { get; }

        IndexStrategy Index { get; }

        System.IO.TextReader Reader { get; }
    }
}