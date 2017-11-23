using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Objects
{
    [Serializable]
    public struct DateTimeRange
    {
        private DateTime? beginDate;
        private DateTime? endDate;

        public DateTime? BeginDate
        {
            get
            {
                return beginDate;
            }
            set
            {
                beginDate = value;

                CheckValidity();
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;

                CheckValidity();
            }
        }

        public TimeSpan? RangeSpan
        {
            get
            {
                if (beginDate != null && endDate != null)
                    return endDate.Value - beginDate.Value;
                else
                    return null;
            }
        }

        public bool IsFiniteRange
        {
            get
            {
                if (beginDate != null && endDate != null)
                    return true;
                else
                    return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (beginDate == null && endDate == null)
                    return true;
                else
                    return false;
            }
        }

        public bool Contains(DateTime date)
        {
            if (this.beginDate.GetValueOrDefault(DateTime.MinValue) <= date && date <= this.endDate.GetValueOrDefault(DateTime.MaxValue))
                return true;
            else
                return false;
        }

        public bool IsOverlap(DateTimeRange range, bool allowTouching = false)
        {
            DateTime thisBeginDate = this.beginDate.GetValueOrDefault(DateTime.MinValue);
            DateTime rangeBeginDate = range.beginDate.GetValueOrDefault(DateTime.MinValue);
            DateTime thisEndDate = this.endDate.GetValueOrDefault(DateTime.MaxValue);
            DateTime rangeEndDate = range.endDate.GetValueOrDefault(DateTime.MaxValue);

            if (allowTouching)
                return (thisBeginDate < rangeEndDate && thisEndDate > rangeBeginDate);
            else
                return (thisBeginDate <= rangeEndDate && thisEndDate >= rangeBeginDate);
        }

        [JsonConstructor]
        public DateTimeRange(DateTime? beginDate = null, DateTime? endDate = null)
        {
            this.beginDate = beginDate;
            this.endDate = endDate;

            this.CheckValidity();
        }

        public DateTimeRange(DateTime beginDate, TimeSpan range)
        {
            this.beginDate = beginDate;
            this.endDate = beginDate.Add(range);

            this.CheckValidity();
        }

        private void CheckValidity()
        {
            if (beginDate != null && endDate != null)
                if (beginDate.Value > endDate.Value)
                    throw new ArgumentException("beginDate cannot be greater than endDate");
        }

        public static bool operator ==(DateTimeRange dt1, DateTimeRange dt2)
        {
            return dt1.Equals(dt2);
        }

        public static bool operator !=(DateTimeRange dt1, DateTimeRange dt2)
        {
            return !dt1.Equals(dt2);
        }

        public override bool Equals(object obj)
        {
            if (obj is DateTimeRange)
            {
                var range = (DateTimeRange)obj;

                if (this.beginDate == range.beginDate && this.endDate == range.endDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:u}-{1:u}", this.beginDate, this.endDate).GetHashCode();
        }
    }
}
