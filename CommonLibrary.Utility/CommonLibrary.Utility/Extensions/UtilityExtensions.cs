using CommonLibrary.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class UtilityExtensions
    {
        public static List<T> GetClone<T>(this IEnumerable<T> list)
            where T : ICloneable
        {
            List<T> copiedList = new List<T>();

            foreach (var item in list)
            {
                copiedList.Add((T)item.Clone());
            }

            return copiedList;
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy. Must be serializable object</param>
        /// <returns>The copied object.</returns>
        public static T CreateClone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (object.ReferenceEquals(source, null))
                return default(T);

            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static TItem GetItem<TKey, TItem>(this KeyedCollection<TKey, TItem> collection, TKey key)
        {
            return (TItem)collection[key];
        }

        public static List<ValidationResult> GetValidationErrors(this IValidatableObject validatableObject)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            var validationContext = new ValidationContext(validatableObject, null, null);
            Validator.TryValidateObject(validatableObject, validationContext, results, true);

            return results;
        }

        /// <summary>
        /// DateTime nesnesinin zamanını 23:59:59.997' a ayarlar
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime SetEndOfDay(this DateTime datetime)
        {
            return datetime.Date.Add(new TimeSpan(0, 23, 59, 59, 997));
        }

        public static DateTime GetBeginningOfYear(this DateTime datetime)
        {
            return new DateTime(datetime.Year, 1, 1);
        }

        public static DateTime SetEndOfYear(this DateTime datetime)
        {
            return new DateTime(datetime.Year + 1, 1, 1).AddDays(-1);
        }

        public static DateTimeRange GetWeek(this DateTime dateTime, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            var begin = dateTime.Date.AddDays(-(int)dateTime.DayOfWeek + (int)firstDayOfWeek);

            return new DateTimeRange(begin, TimeSpan.FromDays(7));
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            if (new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(dateTime.DayOfWeek))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Uri ClearQueryString(this Uri uri)
        {
            string subject = uri.ToString();
            if (!string.IsNullOrWhiteSpace(uri.Query))
            {
                subject = subject.Replace(uri.Query, string.Empty); // QueryString' i sildiriyoruz
            }

            return new Uri(subject);
        }
    }
}