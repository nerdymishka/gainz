using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Runtime.CompilerServices;

namespace NerdyMishka.Data
{
    public static class IDbParameterExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDbDataParameter SetName(this IDbDataParameter parameter, string name)
        {
            var interned = String.IsInterned(name);
            if(interned == null)
                interned = String.Intern(name);

            parameter.ParameterName = interned;
            

            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDbDataParameter SetValue<T>(this IDbDataParameter parameter, T value)
        {
            parameter.Value = value;

            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDbDataParameter SetSize(this IDbDataParameter parameter, int size)
        {
            parameter.Size = size;
            

            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDbDataParameter SetPrecision(this IDbDataParameter parameter, int? precision, int? scale)
        {
            if(precision.HasValue)
                parameter.Precision = (byte)precision.Value;

            if(scale.HasValue)
                parameter.Scale = (byte)scale.Value;

            return parameter;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDbDataParameter SetDirection(this IDbDataParameter parameter, DbType type)
        {
            parameter.DbType = type;
        
            return parameter;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDbDataParameter SetDirection(this IDbDataParameter parameter, ParameterDirection direction)
        {
            
            parameter.Direction = direction;
            

            return parameter;
        }
    }
}
