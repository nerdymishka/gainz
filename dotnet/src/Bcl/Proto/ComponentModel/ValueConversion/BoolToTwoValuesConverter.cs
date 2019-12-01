


using System;
using System.Linq.Expressions;

namespace NerdyMishka.ComponentModel.ValueConversion
{
    public class BoolToTwoValuesConverter<TTo> : ValueConverter<bool, TTo>
    {
        public BoolToTwoValuesConverter(
            TTo falseValue,
            TTo trueValue,
            Expression<Func<TTo, bool>> fromProvider = null)
            : base(ToProvider(falseValue, trueValue), fromProvider ?? ToBool(trueValue))
        {

        }


        private static Expression<Func<bool, TTo>> ToProvider(TTo falseValue, TTo trueValue)
        {
            var param = Expression.Parameter(typeof(bool), "v");
            return Expression.Lambda<Func<bool, TTo>>(
                Expression.Condition(
                    param,
                    Expression.Constant(trueValue, typeof(TTo)),
                    Expression.Constant(falseValue, typeof(TTo))),
                param);
        }

        private static Expression<Func<TTo, bool>> ToBool(TTo trueValue)
        {
            var param = Expression.Parameter(typeof(TTo), "v");
            return Expression.Lambda<Func<TTo, bool>>(
                Expression.Equal(
                    param,
                    Expression.Constant(trueValue, typeof(TTo))),
                param);
        }
    }
}