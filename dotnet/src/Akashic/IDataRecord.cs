using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public interface IDataRecord
    {
        int FieldCount { get; }


        object this[int i] { get; }

        object this[string name] { get; }

        bool IsDbNull(int ordinal);

        Task<bool> IsDbNullAsync(int ordinal);

        Task<bool> IsDbNullAsync(int ordinal, CancellationToken cancellationToken);

        bool IsDbNull(string name);

        Task<bool> IsDbNullAsync(string name);

        Task<bool> IsDbNullAsync(string name, CancellationToken cancellationToken);

        bool GetBoolean(int ordinal);

        bool GetBoolean(string name);

        byte GetByte(int ordinal);

        byte GetByte(string name);

        long GetBytes(int ordinal, long fieldOffset, byte[] buffer, int offset, int length);

        long GetBytes(int ordinal, byte[] buffer);

        long GetBytes(string name, long fieldOffset, byte[] buffer, int offset, int length);

        long GetBytes(string name, byte[] buffer);

        char GetChar(int ordinal);

        char GetChar(string name);

        long GetChars(int ordinal, long fieldOffset, char[] buffer, int offset, int length);

        long GetChars(int ordinal, char[] buffer);

        long GetChars(string name, long fieldOffset, char[] buffer, int offset, int length);

        long GetChars(string name, char[] buffer);

        string GetDataTypeName(int ordinal);

        string GetDataTypeName(string name);

        DateTime GetDateTime(int ordinal);

        DateTime GetDateTime(string name);

        decimal GetDecimal(int ordinal);

        decimal GetDecimal(string name);

        double GetDouble(int ordinal);

        double GetDouble(string name);

        Type GetFieldType(int ordinal);

        Type GetFieldType(string name);

        float GetFloat(int ordinal);

        float GetFloat(string name);

        Guid GetGuid(int ordinal);

        Guid GetGuid(string name);

        short GetInt16(int ordinal);

        short GetInt16(string name);

        int GetInt32(int ordinal);

        int GetInt32(string name);

        long GetInt64(int ordinal);

        long GetInt64(string name);

        string GetName(int ordinal);

        int GetOrdinal(string name);

        string GetString(int ordinal);

        string GetString(string name);

        T GetValueAs<T>(int ordinal);

        T GetValueAs<T>(string name);

        object GetValue(int ordinal);

        object GetValue(string name);

        int GetValues(object[] values);

        Stream GetStream(int ordinal);

        Stream GetStream(string name);

        TextReader GetTextReader(int ordinal);

        TextReader GetTextReader(string name);
    }
}
