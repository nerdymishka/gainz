using System.IO;
using System;

namespace Search.Core.Storage
{
    public interface IBinaryWriter : IDisposable, ICloneable
    {
        long Length { get; }
        
        long Position { get; }
        
        void Write(int value);
        
        void Write(long value);
        
        void Write(byte value);
        
        void Write(byte[] buffer, int index, int length);
        
        void Write(char[] buffer, int index, int length);
        
        void Write(string value);
        
        void WriteVariableLengthInt(int value);
        
        void WriteVariableLengthInt(long value);

        long Seek(long offset);
        
        long Seek(long offset, SeekOrigin origin);

        void Flush();

    }
}