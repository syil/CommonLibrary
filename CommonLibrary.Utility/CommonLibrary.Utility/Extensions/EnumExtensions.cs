using CommonLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class EnumExtensions
    {
        public static T ToNameEquivalent<T>(this Enum source)
             where T : struct, IComparable, IFormattable, IConvertible
        {
            string sourceString = source.ToString();

            return (T)Enum.Parse(typeof(T), sourceString);
        }

        public static T ToValueEquivalent<T>(this Enum source)
             where T : struct, IComparable, IFormattable, IConvertible
        {
            return (T)Enum.ToObject(typeof(T), Convert.ToInt32(source));
        }

        public static string GetStringValue(this Enum enumValue)
        {
            var stringValueAttribute = GetStringValueAttribute(enumValue);
            if (stringValueAttribute != null)
            {
                return stringValueAttribute.StringValue;
            }
            else
            {
                return enumValue.ToString();
            }
        }

        public static bool FromStringValue<T>(string stringValue, out T returnValue, bool caseInsensitive = false)
             where T : struct, IComparable, IFormattable, IConvertible
        {
            CheckEnumType<T>();

            var allValues = GetAllItems<T>();
            foreach (T enumValue in allValues)
            {
                var stringValueAttribute = GetStringValueAttribute(enumValue);

                if (stringValueAttribute != null)
                {
                    if (String.Compare(stringValueAttribute.StringValue, stringValue, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        returnValue = enumValue;
                        return true;
                    }
                }
            }

            returnValue = default(T);
            return false;
        }

        private static StringValueAttribute GetStringValueAttribute(object obj)
        {
            FieldInfo fi = obj.GetType().GetField(obj.ToString());

            if (fi != null)
                return fi.GetCustomAttribute<StringValueAttribute>();
            else
                return null;
        }

        /// <summary>
        /// Eğer Or(|) operatörü ile birleştirilmiş enum gönderilirse, birleştirilen enumların dizisini döner. Eğer tek enum var ise onu döner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static IEnumerable<T> Explode<T>(this T enumValue)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            CheckEnumType<T>();

            int value = Convert.ToInt32(enumValue);
            int pow = (int)Math.Floor(Math.Log(value, 2));
            int powOfTwo;

            do
            {
                powOfTwo = (int)Math.Pow(2, pow--);
                value -= powOfTwo;

                yield return (T)Enum.ToObject(typeof(T), powOfTwo);

            } while (pow >= 0 && value > 0);
        }

        public static T Random<T>()
            where T : struct, IComparable, IFormattable, IConvertible
        {
            CheckEnumType<T>();

            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[new Random(DateTime.Now.Millisecond).Next(0, values.Length)];
        }

        public static List<T> GetAllItems<T>()
            where T : struct, IComparable, IFormattable, IConvertible
        {
            CheckEnumType<T>();

            List<T> returnList = new List<T>();

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                returnList.Add(item);
            }

            return returnList;
        }

        public static T Parse<T>(string value, bool caseInsensitive = false)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            CheckEnumType<T>();

            T enumValue = default(T);
            Enum.TryParse(value, caseInsensitive, out enumValue);

            return enumValue;
        }

        private static void CheckEnumType<T>()
        {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException("Generic type must be Enum type");
        }
    }
}