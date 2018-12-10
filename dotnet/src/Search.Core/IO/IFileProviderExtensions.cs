namespace NerdyMishka.Search.IO
{
    public static class IFileProviderExtensions
    {
        public static IBinaryReader OpenReader(this IFileProvider provider, string name)
        {
            return new BinaryReader(provider.OpenRead(name));
        }

        public static IBinaryWriter OpenWriter(this IFileProvider provider, string name)
        {
            return new BinaryWriter(provider.OpenWrite(name));
        }

         public static BitVector ReadVector(this IFileProvider directory, string fileName)
        {
            using (var reader = directory.OpenReader(fileName))
            {
                var capacity = reader.ReadInt32();
                var numberOfTrueBits = reader.ReadInt32();
                var bytes = new byte[(capacity >> 3) + 1];
                reader.Read(bytes, 0, bytes.Length);
                return new BitVector(
                    capacity,
                    numberOfTrueBits,
                    bytes
                );
            }
        }
    }
}