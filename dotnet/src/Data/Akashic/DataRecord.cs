using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class DataRecord : IDataRecord
    {
        private object[] records;
        private string[] names;

        public DataRecord(string[] names, object[] records)
        {
            this.names = names;
            this.records = records;
        }

        public object this[string name]
        {
            get
            {
                var index = Array.IndexOf(this.names, name);
                return this.records[index];
            }
        }

        public object this[int i]
        {
            get { return this.records[i]; }
        }

        public int FieldCount
        {
            get
            {
                return this.records.Length;
            }
        }

        public bool GetBoolean(string name) =>
            (bool)this.records[Array.IndexOf(this.names, name)];

        public bool GetBoolean(int ordinal) => 
            (bool)this.records[ordinal];

        public byte GetByte(string name) =>
            (byte)this.records[Array.IndexOf(this.names, name)];

        public byte GetByte(int ordinal) => 
            (byte)this.records[ordinal];

        public long GetBytes(string name, byte[] buffer)
            => this.GetBytes(Array.IndexOf(this.names, name), 0, buffer, 0, buffer.Length);

        public long GetBytes(int ordinal, byte[] buffer)
            => this.GetBytes(ordinal, 0, buffer, 0, buffer.Length);

        public long GetBytes(string name, long fieldOffset, byte[] buffer, int offset, int length)
        {
            var value = this.records[Array.IndexOf(this.names, name)];
            var bytes = (byte[])value;

            Array.Copy(bytes, (int)fieldOffset, buffer, offset, length);

            return buffer.Length;
        }

        public long GetBytes(int ordinal, long fieldOffset, byte[] buffer, int offset, int length)
        {
            var value = this.records[ordinal];
            var bytes = (byte[])value;

            Array.Copy(bytes, (int)fieldOffset, buffer, offset, length);

            return buffer.Length;
        }

        public char GetChar(string name)
            => (char)this.records[Array.IndexOf(this.names, name)];

        public char GetChar(int ordinal)
            => (char)this.records[ordinal];

        public long GetChars(string name, char[] buffer)
            => this.GetChars(name, 0, buffer, 0, buffer.Length);

        public long GetChars(int ordinal, char[] buffer)
            => this.GetChars(ordinal, 0, buffer, 0, buffer.Length);

        public long GetChars(string name, long fieldOffset, char[] buffer, int offset, int length)
        {
            var value = this.records[Array.IndexOf(this.names, name)];
            var chars = (char[])value;

            Array.Copy(chars, (int)fieldOffset, buffer, offset, length);

            return buffer.Length;
        }

        public long GetChars(int ordinal, long fieldOffset, char[] buffer, int offset, int length)
        {
            var value = this.records[ordinal];
            var chars = (char[])value;

            Array.Copy(chars, (int)fieldOffset, buffer, offset, length);

            return buffer.Length;
        }

        public string GetDataTypeName(string name)
            => this.records[Array.IndexOf(this.names, name)].GetType().FullName;


        public string GetDataTypeName(int ordinal)
            => this.records[ordinal].GetType().FullName;

        public DateTime GetDateTime(string name)
            => (DateTime)this.records[Array.IndexOf(this.names, name)];

        public DateTime GetDateTime(int ordinal)
            => (DateTime)this.records[ordinal];

        public decimal GetDecimal(string name)
            => (Decimal)this.records[Array.IndexOf(this.names, name)];

        public decimal GetDecimal(int ordinal)
            => (Decimal)this.records[ordinal];

        public double GetDouble(string name)
            => (Double)this.records[Array.IndexOf(this.names, name)];

        public double GetDouble(int ordinal)
            => (Double)this.records[ordinal];

        public Type GetFieldType(string name)
            => this.records[Array.IndexOf(this.names, name)].GetType();

        public Type GetFieldType(int ordinal)
            => this.records[ordinal].GetType();

        public float GetFloat(string name)
            => (float)this.records[Array.IndexOf(this.names, name)];

        public float GetFloat(int ordinal)
             => (float)this.records[ordinal];

        public Guid GetGuid(string name)
             => (Guid)this.records[Array.IndexOf(this.names, name)];

        public Guid GetGuid(int ordinal)
             => (Guid)this.records[ordinal];

        public short GetInt16(string name)
             => (short)this.records[Array.IndexOf(this.names, name)];

        public short GetInt16(int ordinal)
             => (short)this.records[ordinal];

        public int GetInt32(string name)
             => (int)this.records[Array.IndexOf(this.names, name)];

        public int GetInt32(int ordinal)
             => (int)this.records[ordinal];

        public long GetInt64(string name)
             => (long)this.records[Array.IndexOf(this.names, name)];

        public long GetInt64(int ordinal)
             => (long)this.records[ordinal];

        public string GetName(int ordinal)
        {
            return this.names[ordinal];
        }

        public int GetOrdinal(string name)
        {
            return Array.IndexOf(this.names, name);
        }

        public Stream GetStream(string name)
             => (Stream)this.records[Array.IndexOf(this.names, name)];

        public Stream GetStream(int ordinal)
             => (Stream)this.records[ordinal];

        public string GetString(string name)
             => (string)this.records[Array.IndexOf(this.names, name)];

        public string GetString(int ordinal)
             => (string)this.records[ordinal];

        public TextReader GetTextReader(string name)
             => (TextReader)this.records[Array.IndexOf(this.names, name)];

        public TextReader GetTextReader(int ordinal)
             => (TextReader)this.records[ordinal];

        public object GetValue(string name)
            => this.records[Array.IndexOf(this.names, name)];

        public object GetValue(int ordinal)
            => this.records[ordinal];

        public T GetValueAs<T>(string name)
            => (T)this.records[Array.IndexOf(this.names, name)];

        public T GetValueAs<T>(int ordinal)
            => (T)this.records[ordinal];

        public int GetValues(object[] values)
        {
            Array.Copy(this.records, values, values.Length);
            return values.Length;
        }

        public bool IsDbNull(string name)
        {
            return DBNull.Value == this.records[Array.IndexOf(this.names, name)];
        }

        public bool IsDbNull(int ordinal)
        {
            return DBNull.Value == this.records[ordinal];
        }

        public Task<bool> IsDbNullAsync(string name)
            => new Task<bool>(() => this.IsDbNull(name));
        

        public Task<bool> IsDbNullAsync(int ordinal)
            => new Task<bool>(() => this.IsDbNull(ordinal));

        public Task<bool> IsDbNullAsync(string name, CancellationToken cancellationToken)
            => new Task<bool>(() => this.IsDbNull(name), cancellationToken);

        public Task<bool> IsDbNullAsync(int ordinal, CancellationToken cancellationToken)
            => new Task<bool>(() => this.IsDbNull(ordinal), cancellationToken);
    }
}
