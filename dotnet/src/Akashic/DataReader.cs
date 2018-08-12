using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;
using System.Reflection;

namespace NerdyMishka.Data
{
    public class DataReader : IDataReader
    {
        private DbDataReader reader;
        private static readonly Type nullableOfT = typeof(Nullable<>);

        public DataReader(DbDataReader reader)
        {
            this.reader = reader;
        }

        public object this[string name] => this.reader.GetValue(this.reader.GetOrdinal(name));

        public object this[int ordinal] => this.reader.GetValue(ordinal);

        public int Depth => this.reader.Depth;

        public int FieldCount => this.reader.FieldCount;

        public bool HasRows => this.reader.HasRows;

        public bool IsClosed => this.reader.IsClosed;

        public bool GetBoolean(string name) => this.reader.GetBoolean(this.reader.GetOrdinal(name));

        public bool GetBoolean(int ordinal) => this.reader.GetBoolean(ordinal);

        public byte GetByte(string name) => this.reader.GetByte(this.reader.GetOrdinal(name));

        public byte GetByte(int ordinal) => this.reader.GetByte(ordinal);

        public long GetBytes(string name, byte[] buffer)
        {
            return this.reader.GetBytes(this.reader.GetOrdinal(name), 0, buffer, 0, buffer.Length);
        }

        public long GetBytes(int ordinal, byte[] buffer)
        {
            return this.reader.GetBytes(ordinal, 0, buffer, 0,buffer.Length);
        }

        public long GetBytes(string name, long fieldOffset, byte[] buffer, int offset, int length)
        {
            return this.reader.GetBytes(this.reader.GetOrdinal(name), 0, buffer, offset, length);
        }

        public long GetBytes(int ordinal, long fieldOffset, byte[] buffer, int offset, int length)
        {
            return this.reader.GetBytes(ordinal, fieldOffset, buffer, offset, length);
        }

        public char GetChar(string name) => this.reader.GetChar(this.reader.GetOrdinal(name));

        public char GetChar(int ordinal) => this.reader.GetChar(ordinal);

        public long GetChars(string name, char[] buffer)
        {
            return this.reader.GetChars(this.reader.GetOrdinal(name), 0, buffer, 0, buffer.Length);
        }

        public long GetChars(int ordinal, char[] buffer)
        {
            return this.reader.GetChars(ordinal, 0, buffer, 0, buffer.Length);
        }

        public long GetChars(string name, long fieldOffset, char[] buffer, int offset, int length)
        {
            return this.reader.GetChars(this.reader.GetOrdinal(name), 0, buffer, offset, length);
        }

        public long GetChars(int ordinal, long fieldOffset, char[] buffer, int offset, int length)
        {
            return this.reader.GetChars(ordinal, 0, buffer, offset, length);
        }

        public string GetDataTypeName(string name) => this.reader.GetDataTypeName(this.reader.GetOrdinal(name));

        public string GetDataTypeName(int ordinal) => this.reader.GetDataTypeName(ordinal);

        public DateTime GetDateTime(string name) => this.reader.GetDateTime(this.reader.GetOrdinal(name));

        public DateTime GetDateTime(int ordinal) => this.reader.GetDateTime(ordinal);

        public decimal GetDecimal(string name) => this.reader.GetDecimal(this.reader.GetOrdinal(name));

        public decimal GetDecimal(int ordinal) => this.reader.GetDecimal(ordinal);

        public double GetDouble(string name) => this.reader.GetDouble(this.reader.GetOrdinal(name));

        public double GetDouble(int ordinal) => this.reader.GetDouble(ordinal);

        public IEnumerator<IDataRecord> GetEnumerator()
        {
            string[] names = null;
            while (this.Read())
            {
                if(names == null)
                {
                    var list = new List<string>();
                    int i = 0,
                        l = this.FieldCount;

                    while(i < l)
                    {
                        list.Add(this.GetName(i));
                        i++;
                    }

                    names = list.ToArray();
                }

                var values = new object[this.FieldCount];
                this.GetValues(values);

                yield return new DataRecord(names, values);
            }
        }

        public Type GetFieldType(string name) => this.reader.GetFieldType(this.reader.GetOrdinal(name));

        public Type GetFieldType(int ordinal) => this.reader.GetFieldType(ordinal);

        public float GetFloat(string name) => this.reader.GetFloat(this.reader.GetOrdinal(name));

        public float GetFloat(int ordinal) => this.reader.GetFloat(ordinal);

        public Guid GetGuid(string name) => this.reader.GetGuid(this.reader.GetOrdinal(name));

        public Guid GetGuid(int ordinal) => this.reader.GetGuid(ordinal);

        public short GetInt16(string name) => this.reader.GetInt16(this.reader.GetOrdinal(name));

        public short GetInt16(int ordinal) => this.reader.GetInt16(ordinal);

        public int GetInt32(string name) => this.reader.GetInt32(this.reader.GetOrdinal(name));

        public int GetInt32(int ordinal) => this.reader.GetInt32(ordinal);

        public long GetInt64(string name) => this.reader.GetInt64(this.reader.GetOrdinal(name));

        public long GetInt64(int ordinal) => this.reader.GetInt64(ordinal);

        public string GetName(int ordinal) => this.reader.GetName(ordinal);

        public int GetOrdinal(string name) => this.reader.GetOrdinal(name);

        public Stream GetStream(string name) => this.reader.GetStream(this.reader.GetOrdinal(name));

        public Stream GetStream(int ordinal) => this.reader.GetStream(ordinal);

        public string GetString(string name) => this.reader.GetString(this.reader.GetOrdinal(name));

        public string GetString(int ordinal) => this.reader.GetString(ordinal);

        public TextReader GetTextReader(string name) => this.reader.GetTextReader(this.reader.GetOrdinal(name));

        public TextReader GetTextReader(int ordinal) => this.reader.GetTextReader(ordinal);

        public object GetValue(string name) => this.reader.GetValue(this.reader.GetOrdinal(name));

        public object GetValue(int ordinal) => this.reader.GetValue(ordinal);

        public T GetValueAs<T>(string name)
        {
            var value = this.reader.GetValue(this.reader.GetOrdinal(name));
            if (value is DBNull)
                return default(T);

            return (T)value;
        }

        public T GetValueAs<T>(int ordinal)
        {
            var value = this.reader.GetValue(ordinal);
            if (value is DBNull)
                return default(T);

            return (T)value;
        }

        public int GetValues(object[] values) => this.reader.GetValues(values);

        public bool IsDbNull(string name) => this.reader.IsDBNull(this.GetOrdinal(name));

        public bool IsDbNull(int ordinal) => this.reader.IsDBNull(ordinal);

        public Task<bool> IsDbNullAsync(string name) => this.reader.IsDBNullAsync(this.reader.GetOrdinal(name));

        public Task<bool> IsDbNullAsync(int ordinal) => this.reader.IsDBNullAsync(ordinal);

        public Task<bool> IsDbNullAsync(string name, CancellationToken cancellationToken)
        {
            return this.reader.IsDBNullAsync(this.reader.GetOrdinal(name), cancellationToken);
        }

        public Task<bool> IsDbNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            return this.reader.IsDBNullAsync(ordinal, cancellationToken);
        }

        public bool NextResult() => this.reader.NextResult();

        public Task<bool> NextResultAsync() => this.reader.NextResultAsync();

        public Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return this.reader.NextResultAsync(cancellationToken);
        }

        public bool Read() => this.reader.Read();

        public Task<bool> ReadAsync() => this.reader.ReadAsync();

        public Task<bool> ReadAsync(CancellationToken cancellationToken) => this.reader.ReadAsync(cancellationToken);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.reader.Dispose();
                    this.reader = null;
                }
                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
