

using System;
using System.Linq.Expressions;

namespace NerdyMishka.ComponentModel.ValueConversion
{
   
    public class StringToGuidConverter: GuidAndStringConverter<string, Guid>
    {

        public StringToGuidConverter() : base(ToGuid(), ToString())
        {

        }
    }

    public class GuidToStringConverter : GuidAndStringConverter<Guid, string>
    {
        public GuidToStringConverter() : base(ToString(), ToGuid())
        {

        }
    }


    public abstract class GuidAndStringConverter<TFrom, TTo> : ValueConverter<TFrom, TTo>
    {
        public GuidAndStringConverter(
            Expression<Func<TFrom, TTo>> convertToProviderExpression, Expression<Func<TTo, TFrom>> convertFromProviderExpression) : base(convertToProviderExpression, convertFromProviderExpression)
        {
        }

        public static Expression<Func<string, Guid>> ToGuid() => (v) => v == null ? default : new Guid(v); 

        public static new Expression<Func<Guid, string>> ToString() => (v) => v.ToString("D");
    }
}