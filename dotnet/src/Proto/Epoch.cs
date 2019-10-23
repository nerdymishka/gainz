using System;

namespace NerdyMishka
{
    public struct Epoch
    {
        private readonly static int s_dataLength = 
            (1000L*365*24*60*60*1000).ToString(LongUtil.CharacterMaxRadix).Length;

        private long value;
        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;
        private int millisecond;

        public Epoch(long timestamp) {
            this.value = 0;
            this.year = 0;
            this.month = 0;
            this.day = 0;
            this.hour = 0;
            this.minute = 0;
            this.second = 0;
            this.millisecond = 0;
            this.Info = null;

            this.Value = timestamp;
        }

        public Epoch(DateTime dateTime) : 
            this(dateTime.ToUnixTimeStamp()) {

        }
        
        public long Value 
        {
            get => this.value;
            set {
                this.value = value;

                // feels bad.  
                var str = this.value.ToString();
                int milliseconds = 0;
                if(str.Length > 10)
                {
                    var millisecondStr = str.Substring(str.Length - 3).TrimStart('0');
                   
                    if(millisecondStr.Length > 0)
                        milliseconds = int.Parse(millisecondStr);

                    this.value = value - milliseconds;
                    this.value = value / 1000;
                }
                

                this.millisecond = milliseconds;

                this.GetDatePart(
                    out this.year, 
                    out this.month, 
                    out this.day);

                this.GetTimePart(
                    out this.hour,
                    out this.minute,
                    out this.second
                );
            }
        }


        public int Year => this.year;

        public int Month => this.month;

        public int Day => this.day;

        public int Hour => this.hour;

        public int Minute => this.minute;

        public int Second => this.second;

        public int Millisecond => this.millisecond;

        public bool IsLeapYear => DateTime.IsLeapYear(this.year);

        private System.Globalization.CultureInfo Info { get; set; }


        public static implicit operator DateTime(Epoch value)
        {
            return ToDateTime(value);
        }

        public static implicit operator Epoch(long value)
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than zero");

            if(value.ToString().Length > s_dataLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"value must be less than or equal to {s_dataLength}");



            return new Epoch() {
                Value = value
            };
        }

        public static implicit operator Epoch(DateTime value)
        {
            return FromDateTime(value);
        }

        /* TODO: implement Epoch time modification methods
        public Epoch AddDays(int days)
        {
        
            // 24 * 60 * 60 * 1000
            return new Epoch() {
                Value = this.Value + (days * 86400000) 
            };
        }

        public Epoch AddMinutes(int minutes)
        {   
            // 60 * 60 * 1000
            return new Epoch() {
                Value = this.Value + (minutes * 1200000)
            };
        }

        public Epoch AddMonths(int months)
        {
            // based on .NET's AddMonths.  
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
            return new Epoch() {
                Value = this.Value + diff
            };
        } */

        

        public void GetTimePart(out int hour, out int minutes, out int seconds)
        {
            //http://git.musl-libc.org/cgit/musl/tree/src/time/__secs_to_tm.c?h=v0.9.15
            // MIT
        
            //
            const int y2k = (946684800 + 86400*(31+29));

            var secs = (this.Value) - y2k;
            var remainder = secs % 86400;
            if(remainder < 0)
            {
                remainder += 86400;
            }

            hour = (int)(remainder / 3600);
            minutes = (int)(remainder / 60 % 60);
            seconds = (int)(remainder % 60);
        }

        public void GetDatePart(out int year, out int month, out int day)
        {
            // from https://stackoverflow.com/questions/7136385/calculate-day-number-from-an-unix-timestamp-in-a-math-way
            // http://howardhinnant.github.io/date_algorithms.html#civil_from_days
            
            
            // civil_from_days doesn't account for that.
            long s = this.Value;
            // convert Epochh from 1970-01-01 to 0000-03-01
            long z = s / 86400 + 719468;
            long era = (z >= 0 ? z : (z - 146096)) / 146097;

            long dayOfEra = (long)(z - era * 146097);
            long yearOfEra = (long)(dayOfEra - dayOfEra/1460 + dayOfEra/36524 - dayOfEra/146096) / 365;
            year = (int)((long)yearOfEra + era * 400);
            int dayOfYear = (int)(dayOfEra - (365 * yearOfEra + (yearOfEra/4) - (yearOfEra/100)));
            
            // mp = month from day of year.
            int mp = (5*dayOfYear + 2)/153;
            day = dayOfYear - (153 * mp +2) / 5 + 1;
            month = mp + (mp < 10 ? 3 : -9);
            year += (month <= 2 ? 1 : 0);
        }
    
        public static Epoch Parse(
            string value, 
            int toBase = 10, 
            bool hasMilliseconds = true)
        {
            if(!hasMilliseconds)
                value += "000";

            var timestamp = LongUtil.Parse(value, toBase);

            return new Epoch() {
                Value = timestamp
            };
        }

        public DateTime ToDateTime(DateTimeKind kind = DateTimeKind.Utc)
        {
            return ToDateTime(this, kind);
        }

        public static DateTime ToDateTime(Epoch value, DateTimeKind kind = DateTimeKind.Utc)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour,value.Minute, value.Second, value.Millisecond, kind);
        } 


        public static Epoch FromDateTime(DateTime dateTime)
        {
            return new Epoch() {
                Value = dateTime.ToUnixTimeStamp()
            };
        }

        public long ToTimestampWithMiliseconds()
        {
            return this.Value * 1000 + this.millisecond;
        }

        public long ToTimestamp()
        {
            return this.Value;
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