

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace NerdyMishka.ComponentModel.ValueConversion
{


    public abstract class ValueConverter<TFrom, TTo> : ValueConverter
    {
        private Func<TFrom, TTo> convertTo;

        private Func<TTo, TFrom> convertFrom;

        public override Type FromType => typeof(TFrom);

        public override Type ToType => typeof(TTo);

        public ValueConverter(
            Expression<Func<TFrom, TTo>> convertToProviderExpression,
            Expression<Func<TTo, TFrom>> convertFromProviderExpression
        )
        {
            this.convertTo = convertToProviderExpression.Compile();
            this.convertFrom = convertFromProviderExpression.Compile();
        }

        public override bool CanConvertFrom(Type type)
        {
            return typeof(TFrom) == type;
        }

        public override bool CanConvertTo(Type type)
        {
            return typeof(TTo) == type;
        }

        public override object ConvertFrom(object value)
        {
            return this.convertFrom((TTo)value);
        }

        public override object ConvertTo(object value)
        {
            return this.convertTo((TFrom)value);
        }
    }

    public abstract class ValueConverter 
    {

        public virtual Type FromType { get; }

        public virtual Type ToType { get; }

        public abstract bool CanConvertFrom(Type type);

        public abstract bool CanConvertTo(Type type);

        public abstract object ConvertFrom(object value);

        public abstract object ConvertTo(object value);
       
    }
}