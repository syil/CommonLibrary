using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class StringExtensions
    {
        public static string StripHtmlTags(this string value)
        {
            string noHTML = Regex.Replace(value, @"<[^>]+>", "").Trim();
            string noHTMLNormalised = Regex.Replace(noHTML, @"\s{2,}", " ");

            return noHTMLNormalised;
        }

        public static string StripHtmlTags(this string value, string tagToStriped)
        {
            string noHTML = Regex.Replace(value, @"<\/?" + tagToStriped + "[^>]*>", "").Trim();
            string noHTMLNormalised = Regex.Replace(noHTML, @"\s{2,}", " ");

            return noHTMLNormalised;
        }

        public static string StripHtmlTags(this string value, params string[] tagsToStriped)
        {
            string noHTML = Regex.Replace(value, @"<\/?(" + string.Join("|", tagsToStriped) + ")[^>]*>", "").Trim();
            string noHTMLNormalised = Regex.Replace(noHTML, @"\s{2,}", " ");

            return noHTMLNormalised;
        }

        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.ReplaceTurkishCharacters().ToLowerInvariant();

            str = str.Replace('/', '-');
            str = Regex.Replace(str, "[^a-z0-9\\s-]", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "\\s+", " ").Trim();
            str = Regex.Replace(str, "\\s", "-");
            str = str.Replace("--", "");

            return str;
        }

        public static string NewLineToBR(this string phrase, string newLine = "\r\n")
        {
            string str = phrase;
            str = str.Replace(newLine, "<br />");
            str = str.Replace(newLine, "<br />");

            return str;
        }

        public static string ToMeaningfulString(this TimeSpan t, MeaningfulStringOptions options = MeaningfulStringOptions.Default)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> partNames = new Dictionary<string, string>();

            if (options.HasFlag(MeaningfulStringOptions.FullNames))
            {
                partNames.Add("Day", "Gün");
                partNames.Add("Hour", "Saat");
                partNames.Add("Minute", "Dakika");
                partNames.Add("Second", "Saniye");
            }
            else
            {
                partNames.Add("Day", "Gün");
                partNames.Add("Hour", "Sa");
                partNames.Add("Minute", "Dk");
                partNames.Add("Second", "Sn");
            }

            if (t.TotalDays >= 1)
            {
                sb.AppendFormat("{0:%d} {1}  ", t, partNames["Day"]);

                if (options.HasFlag(MeaningfulStringOptions.ApproximateTime))
                    return sb.ToString();
            }

            if (t.TotalHours >= 1)
            {
                sb.AppendFormat("{0:%h} {1}  ", t, partNames["Hour"]);

                if (options.HasFlag(MeaningfulStringOptions.ApproximateTime))
                    return sb.ToString();
            }

            if (t.TotalMinutes >= 1)
            {
                sb.AppendFormat("{0:%m} {1}  ", t, partNames["Minute"]);

                if (options.HasFlag(MeaningfulStringOptions.ApproximateTime))
                    return sb.ToString();
            }

            if (t.Seconds > 0)
                sb.AppendFormat("{0:%s} {1}", t, partNames["Second"]);

            return sb.ToString();
        }

        public static string ToRelativeSpanString(this DateTime originDate, DateTime? relativeDate = null)
        {
            if (relativeDate == null)
                relativeDate = DateTime.Now;

            TimeSpan span;
            string suffix;

            if (relativeDate.Value > originDate)
            {
                span = relativeDate.Value - originDate;
                suffix = "önce";
            }
            else
            {
                span = originDate - relativeDate.Value;
                suffix = "sonra";
            }

            return string.Format("{0} {1}", span.ToMeaningfulString(MeaningfulStringOptions.FullNames | MeaningfulStringOptions.ApproximateTime), suffix);
        }

        public static string ToString<T>(this T? nullableObject, string format, string nullString = "")
            where T : struct, IFormattable
        {
            if (nullableObject != null)
            {
                return nullableObject.Value.ToString(format, null);
            }
            else
            {
                return nullString;
            }
        }

        public static string ToString<T>(this T? nullableObject, IFormatProvider formatProvider, string nullString = "")
            where T : struct, IFormattable
        {
            if (nullableObject != null)
            {
                return nullableObject.Value.ToString(null, formatProvider);
            }
            else
            {
                return nullString;
            }
        }

        public static string ToString<T>(this T? nullableObject, string format, IFormatProvider formatProvider, string nullString = "")
            where T : struct, IFormattable
        {
            if (nullableObject != null)
            {
                return nullableObject.Value.ToString(format, formatProvider);
            }
            else
            {
                return nullString;
            }
        }

        public static string DotDotDot(this string value, int characterLimit, string append = "...", StringBreakType breakType = StringBreakType.Word)
        {
            return DotDotDot(value, 0, characterLimit,
                append: append,
                breakType: breakType);
        }

        public static string DotDotDot(this string value, int offset, int characterLimit, string append = "...", string prepend = "... ", StringBreakType breakType = StringBreakType.Word)
        {
            char[] seperator;

            switch (breakType)
            {
                case StringBreakType.Sentence:
                    seperator = new char[] { '.' };
                    break;
                case StringBreakType.Word:
                default:
                    seperator = new char[] { ' ' };
                    break;
            }

            int actualCharacterLimit = characterLimit - append.Length;

            if (actualCharacterLimit >= value.Length)
                return value;

            if (offset < 0)
                offset = 0;

            string[] sliceList = value.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder trimmedSentenceBuilder = new StringBuilder();

            if (offset > 0)
                trimmedSentenceBuilder.Append(prepend);

            if (sliceList.Any())
            {
                int i = 0;
                do
                {
                    if (offset > 0)
                    {
                        offset -= sliceList[i].Length + 1; // +1 ayraç için
                        continue;
                    }

                    trimmedSentenceBuilder.Append(sliceList[i]).Append(seperator[0]);
                } while ((++i < sliceList.Length) && (trimmedSentenceBuilder.Length + sliceList[i].Length) < actualCharacterLimit);

                trimmedSentenceBuilder.Append(append);
                return trimmedSentenceBuilder.ToString();
            }
            else
            {
                return value;
            }
        }

        public static string ReplaceTurkishCharacters(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            string retval = value.Trim();
            retval = retval.Replace("Ç", "C");
            retval = retval.Replace("ç", "c");
            retval = retval.Replace("Ğ", "G");
            retval = retval.Replace("ğ", "g");
            retval = retval.Replace("I", "I");
            retval = retval.Replace("ı", "i");
            retval = retval.Replace("İ", "I");
            retval = retval.Replace("Ö", "O");
            retval = retval.Replace("ö", "o");
            retval = retval.Replace("Ş", "S");
            retval = retval.Replace("ş", "s");
            retval = retval.Replace("Ü", "U");
            retval = retval.Replace("ü", "u");

            return retval;
        }

        public static string ToTitleCase(this string value, TitleCase titleCase = TitleCase.All)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;

            value = value.ToLower();
            switch (titleCase)
            {
                case TitleCase.First:
                    var strArray = value.Split(' ');
                    if (strArray.Length > 1)
                    {
                        strArray[0] = cultureInfo.TextInfo.ToTitleCase(strArray[0]);
                        return string.Join(" ", strArray);
                    }
                    break;
                case TitleCase.All:
                default:
                    return cultureInfo.TextInfo.ToTitleCase(value);
            }

            return value;
        }

        public static string NumericsOnly(this string str)
        {
            return Regex.Replace(str, "[^0-9]", string.Empty);
        }

        public static string InputFilter(this string value, FilterType filter, params string[] whitelistCharacters)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            string sqlInjectionFilterPattern = @"[|&;$%@'""()<>+,\r\n\\]*";
            string xssFilterPattern = @"[<>""'%;()&+]*";

            foreach (string @char in whitelistCharacters)
            {
                sqlInjectionFilterPattern = sqlInjectionFilterPattern.Replace(@char, "");
                xssFilterPattern = xssFilterPattern.Replace(@char, "");
            }

            string filteredValue = value;

            if (filter.HasFlag(FilterType.SqlInjection))
            {
                filteredValue = Regex.Replace(filteredValue, sqlInjectionFilterPattern, "");
            }

            if (filter.HasFlag(FilterType.XSS))
            {
                filteredValue = Regex.Replace(filteredValue, xssFilterPattern, "");
            }

            return filteredValue;
        }

        public static string ToBase64String(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        [Flags]
        public enum MeaningfulStringOptions
        {
            /// <summary>
            /// Varsayılan ayarlar: Tam zaman baz alınır ve kısaltmalar kullanılır
            /// </summary>
            Default = 0,
            /// <summary>
            /// Geri dönen string yaklaşık zamanı verir
            /// </summary>
            ApproximateTime = 1,
            /// <summary>
            /// Geri dönen string' te kısaltmalar kullanılmaz
            /// </summary>
            FullNames = 2
        }

        public enum TitleCase
        {
            First,
            All
        }

        public enum StringBreakType
        {
            Word,
            Sentence
        }

        [Flags]
        public enum FilterType
        {
            None = 0,
            XSS = 1,
            SqlInjection = 2,
            AllFilters = 3
        }
    }
}