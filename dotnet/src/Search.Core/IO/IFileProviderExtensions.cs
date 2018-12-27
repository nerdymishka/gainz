using System.Collections.Generic;

namespace NerdyMishka.Search.IO
{
    public static class IFileProviderExtensions
    {
        private static readonly Dictionary<IFileProvider, object> syncLocks = new Dictionary<IFileProvider, object>();

        public static object GetOrAddSyncLock(this IFileProvider provider)
        {
            if(syncLocks.TryGetValue(provider, out object syncLock))
                return syncLock;

            syncLocks.Add(provider, new object());
            return syncLocks[provider];
        }

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


        public static void WriteVector(this IFileProvider directory, string fileName, BitVector vector)
        {
            using (var writer = directory.OpenWriter(fileName))
            {
              
              
                writer.Write(vector.Length);
                writer.Write(vector.Sum);
                var bytes = vector.ToArray();
                writer.Write(bytes, 0, bytes.Length);
                
            }
        }
    }
}