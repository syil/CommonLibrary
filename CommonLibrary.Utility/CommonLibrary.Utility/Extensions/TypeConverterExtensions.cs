using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class TypeConverterExtensions
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public static DateTime? ToDateTime(this DateTimeOffset? offset)
        {
            if (offset.HasValue)
            {
                return offset.Value.DateTime.ToLocalTime();
            }

            return null;
        }

        public static DateTime ToDateTime(this DateTimeOffset offset)
        {
            return offset.DateTime.ToLocalTime();
        }

        public static DateTime ToDateTime(this TimeSpan timespan)
        {
            return DateTime.Today.Add(timespan);
        }

        [Obsolete("Bi zahmet kendin yap, herşeyi benden bekleme")]
        public static int ToInt32(this string str, int defaultValue = default(int))
        {
            int i = defaultValue;
            int.TryParse(str, out i);
            
            return i;
        }

        public static byte ToByte(this bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date - Epoch).TotalSeconds);
        }

        public static long ToUnixTimeWithMs(this DateTime date)
        {
            return Convert.ToInt64((date - Epoch).TotalMilliseconds);
        }
    }
}