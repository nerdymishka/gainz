using System;

namespace NerdyMishka
{
    public struct Epoc
    {
        private readonly static int s_dataLength = 
            (1000L*365*24*60*60*1000).ToString(LongUtil.CharacterMaxRadix).Length;
        
        public long Value { get; private set; }


        private System.Globalization.CultureInfo Info { get; set; }


        public static implicit operator DateTime(Epoc value)
        {
            return ToDateTime(value);
        }

        public static implicit operator Epoc(long value)
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than zero");

            if(value.ToString().Length > s_dataLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"value must be less than or equal to {s_dataLength}");


            return new Epoc() {
                Value = value
            };
        }

        public static implicit operator Epoc(DateTime value)
        {
            return FromDateTime(value);
        }


        public Epoc AddDays(int days)
        {
            // 24 * 60 * 60 * 1000
            return new Epoc() {
                Value = this.Value + (days * 86400000) 
            };
        }

        public Epoc AddMinutes(int minutes)
        {   
            // 60 * 60 * 1000
            return new Epoc() {
                Value = this.Value + (minutes * 1200000)
            };
        }

        public Epoc AddMonths(int months)
        {
        

            int y, m, d;
            GetDatePart(out y, out m, out d);
 
            int i = m - 1 + months;
            if (i >= 0) {
                m = i % 12 + 1;
                y = y + i / 12;
            }
            else {
                m = 12 + (i + 1) % 12;
                y = y + (i - 11) / 12;
            }

            var days = DateTime.DaysInMonth(y, m);
            if(d > days)
                d = days;

            var diff = new DateTime(y, m, d).ToUnixTimeStamp() - this.Value;
            return new Epoc() {
                Value = this.Value + diff
            };
        }

        

        private bool IsLeapYear(int year)
        {
            return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
        }

        public void GetDatePart(out int year, out int month, out int day)
        {
            // from https://stackoverflow.com/questions/7136385/calculate-day-number-from-an-unix-timestamp-in-a-math-way
            // http://howardhinnant.github.io/date_algorithms.html#civil_from_days
            
            // this time included miliseconds... so remove miliseconds 
            // civil_from_days doesn't account for that.
            long s = this.Value / 1000;
            // convert epoch from 1970-01-01 to 0000-03-01
            long z = s / 86400 + 719468;
            long era = (z >= 0 ? z : (z - 146096)) / 146097;

            long dayOfEra = (long)(z - era * 146097);
            long yearOfEra = (long)(dayOfEra - dayOfEra/1460 + dayOfEra/36524 - dayOfEra/146096) / 365;
            year = (int)((long)yearOfEra + era * 400);
            int dayOfYear = (int)(dayOfEra - (365 * yearOfEra + (yearOfEra/4) - (yearOfEra/100)));
            
            // mp = month from day of year.
            int mp = (5*dayOfYear + 2)/153;
            day = dayOfYear - (153 * mp +2);
            month = mp + (mp < 10 ? 3 : -9);
            year += (month <= 2 ? 1 : 0);
        }
    
        public static Epoc Parse(
            string value, 
            int toBase = 10, 
            bool hasMilliseconds = true)
        {
            if(!hasMilliseconds)
                value += "000";

            var timestamp = LongUtil.Parse(value, toBase);

            return new Epoc() {
                Value = timestamp
            };
        }

        public DateTime ToDateTime(DateTimeKind kind = DateTimeKind.Unspecified)
        {
            return ToDateTime(this, kind);
        }

        public static DateTime ToDateTime(Epoc value, DateTimeKind kind = DateTimeKind.Unspecified)
        {
            var dt = value.Value.FromUnixTimeStamp();
            switch(kind)
            {
                case DateTimeKind.Local:
                    return dt.ToLocalTime();
                case DateTimeKind.Utc:
                    return dt.ToUniversalTime();
                default:
                    return dt;
            }
        } 

        public static Epoc FromDateTime(DateTime dateTime)
        {
            return FromDateTime(dateTime, DateTimeKind.Unspecified);
        }

        public static Epoc FromDateTime(DateTime dateTime, DateTimeKind kind = DateTimeKind.Unspecified)
        {
            if(kind == DateTimeKind.Utc)
                dateTime = dateTime.ToUniversalTime();

            if(kind == DateTimeKind.Local)
                dateTime = dateTime.ToLocalTime();

            return new Epoc() {
                Value = dateTime.ToUnixTimeStamp()
            };
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder(
                this.Value.ToString(LongUtil.CharacterMaxRadix)
            );
            
            if(sb.Length > s_dataLength)
                throw new ArgumentOutOfRangeException(nameof(this.Value), 
                    $"must be have a length less than {s_dataLength}");

            while(sb.Length < s_dataLength)
                sb.Insert(0, '0');

            return sb.ToString();
        }
    }
}