using System;

namespace Search.Core.Storage
{
    public interface IBinaryReader : IDisposable, ICloneable
    {
        long Length { get; }
        long Position { get; }
        int ReadInt32();
        long ReadInt64();
        byte ReadByte();
        int Read(byte[] buffer, int index, int length);
        int Read(char[] buffer, int index, int length);
        int ReadVariableLengthInt32();
        long ReadVariableLengthInt64();
        string ReadString();
        long Seek(long offset);
        long Seek(long offset, SeekOrigin origin);
    }

    }
}