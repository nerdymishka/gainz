using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NerdyMishka.KeePass
{
    public static class Util
    {

     

        public static void Write(this Stream stream, uint value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public static void WriteHeader(this Stream stream, KeePassFileHeaderFields field, byte[] data)
        {
            stream.WriteByte((byte)field);
            ushort size = (ushort)data.Length;
            stream.Write(BitConverter.GetBytes(size));

            stream.Write(data);
        }

        public static void ReadByteTo(this Stream stream, Stream destination)
        {
            int result = stream.ReadByte();
            if (result == -1)
                throw new EndOfStreamException();

            destination.WriteByte((byte)result);
        }

        public static void ReadBytesTo(this Stream stream, int count, Stream destination)
        { 
            // TODO: write more effecient method
           
            byte[] results = new byte[count];
            int offset = 0;
            while (count > 0)
            {
                int bitsRead = stream.Read(results, offset, count);

                if (bitsRead == 0)
                    throw new EndOfStreamException();

                destination.Write(results, offset, count);
                if (bitsRead == 0)
                    break;

                offset += bitsRead;
                count -= bitsRead;
            }
        }

        public static byte[] ToSHA256Hash(this byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(bytes);
            }
        }

        public static byte[] ToSHA256Hash(this MemoryStream stream)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(stream.ToArray());
            }
        }

        public static Int64 ToInt64(this byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }

        public static Int32 ToInt32(this byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        internal static ushort ToUShort(this byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }

        internal static uint ToUInt(this byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static T[] Clear<T>(this T[] value)
        {
            Array.Clear(value, 0, value.Length);
            return value;
        }

        public static int Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
            return buffer.Length;
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] results = new byte[count];
            int offset = 0;
            while (count > 0)
            {
                int bitsRead = stream.Read(results, offset, count);
                if (bitsRead == 0)
                    break;

                offset += bitsRead;
                count -= bitsRead;
            }

            if (offset != results.Length)
            {
                byte[] part = new byte[offset];
                Array.Copy(results, part, offset);
                return part;
            }

            return results;
        }


       
       

        public static string ToHexString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            int bit;
            for (int i = 0; i < bytes.Length; i++)
            {
                bit = bytes[i] >> 4;
                chars[i * 2] = (char)(55 + bit + (((bit - 10) >> 31) & -7));
                bit = bytes[i] & 0xF;
                chars[i * 2 + 1] = (char)(55 + bit + (((bit - 10) >> 31) & -7));
            }
            return new string(chars);
        }
    }
}
