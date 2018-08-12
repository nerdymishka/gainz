using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Akashic {
/* 
    public struct RelationalSqlFragment : IRelationalSqlFragment
    {
        public static implicit operator RelationalSqlFragment(bool value)
        {
            var fragment = new RelationalSqlFragment();
            fragment.ClrValue = value;
            fragment.SqlValue = value == true ? "1" : "0";

            return fragment;
        }

        public static implicit operator RelationalSqlFragment(short value)
        {
            return new RelationalSqlFragment()
            {
                ClrValue = value,
                SqlValue = value.ToString()
            };
        }

        public static implicit operator RelationalSqlFragment(int value)
        {
            return new RelationalSqlFragment()
            {
                ClrValue = value,
                SqlValue = value.ToString()
            };
        }

        public static implicit operator RelationalSqlFragment(long value)
        {
            return new RelationalSqlFragment()
            {
                ClrValue = value,
                SqlValue = value.ToString()
            };
        }

        public object ClrValue { get; set; }

        public string SqlValue { get; set; }
    }
    */
}
