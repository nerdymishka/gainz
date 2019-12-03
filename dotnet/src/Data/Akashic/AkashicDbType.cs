using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Data
{
    public enum AkashicDbType
    {
        /// <summary>
        /// A variable-length stream of non-Unicode characters ranging between 1 and 8,000 characters.
        /// </summary>
        AnsiString,

        /// <summary>
        /// A fixed-length stream of non-Unicode characters.
        /// </summary>
        AnsiStringFixedLength,


        /// <summary>
        /// A variable-length stream of binary data ranging between 1 and 8,000 bytes.
        /// </summary>
        Binary,

        /// <summary>
        /// A simple type representing Boolean values of true or false.
        /// </summary>
        Boolean,


        /// <summary>
        /// An 8-bit unsigned integer ranging in value from 0 to 255.
        /// </summary>
        Byte,

        /// <summary>
        /// A currency value ranging from -2 63 (or -922,337,203,685,477.5808) to 2 63 -1 (or +922,337,203,685,477.5807) with an accuracy to a ten-thousandth of a currency unit.
        /// </summary>
        Currency,

        /// <summary>
        /// A type representing a date value.
        /// </summary>
        Date,

        /// <summary>
        /// A type representing a date and time value.
        /// </summary>
        DateTime,

        /// <summary>
        /// Date and time data. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds.
        /// </summary>
        DateTime2,

        /// <summary>
        /// Date and time data with time zone awareness. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds. Time zone value range is -14:00 through +14:00.
        /// </summary>
        DateTimeOffset,

        /// <summary>
        /// A simple type representing values ranging from 1.0 x 10 -28 to approximately 7.9 x 10 28 with 28-29 significant digits.
        /// </summary>
        Decimal,

        /// <summary>
        /// A floating point type representing values ranging from approximately 5.0 x 10 -324 to 1.7 x 10 308 with a precision of 15-16 digits.
        /// </summary>
        Double,

        /// <summary>
        /// A globally unique identifier (or GUID).
        /// </summary>
        Guid,

        /// <summary>
        /// An integral type representing signed 16-bit integers with values between -32768 and 32767.
        /// </summary>
        Int16,

        /// <summary>
        /// An integral type representing signed 32-bit integers with values between -2147483648 and 2147483647.
        /// </summary>
        Int32,

        /// <summary>
        /// An integral type representing signed 64-bit integers with values between -9223372036854775808 and 9223372036854775807.
        /// </summary>
        Int64,

        /// <summary>
        /// 
        /// </summary>
        Json,

        /// <summary>
        /// A general type representing any reference or value type not explicitly represented by another DbType value.
        /// </summary>
        Object,

        /// <summary>
        /// An integral type representing signed 8-bit integers with values between -128 and 127.
        /// </summary>
        SByte,

        /// <summary>
        /// A floating point type representing values ranging from approximately 1.5 x 10 -45 to 3.4 x 10 38 with a precision of 7 digits.
        /// </summary>
        Single,

        /// <summary>
        /// A type representing Unicode character strings.
        /// </summary>
        String,

        /// <summary>
        /// A fixed-length string of Unicode characters.
        /// </summary>
        StringFixedLength,

        /// <summary>
        /// A type representing a SQL Server DateTime value. If you want to use a SQL Server time value, use Time.
        /// </summary>
        Time,

        /// <summary>
        /// A type representing a SQL Server DateTime value. If you want to use a SQL Server time value, use Time.
        /// </summary>
        TimeStamp,

        /// <summary>
        /// An integral type representing unsigned 16-bit integers with values between 0 and 65535.
        /// </summary>
        UInt16,

        /// <summary>
        /// An integral type representing unsigned 32-bit integers with values between 0 and 4294967295.
        /// </summary>
        UInt32,

        /// <summary>
        /// An integral type representing unsigned 64-bit integers with values between 0 and 18446744073709551615.
        /// </summary>
        UInt64,

        /// <summary>
        /// A variable-length numeric value.
        /// </summary>
        VarNumeric,

        Xml
    }
}
