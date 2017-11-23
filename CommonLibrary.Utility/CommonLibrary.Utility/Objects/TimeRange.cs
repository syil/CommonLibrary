using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Objects
{
    [Serializable]
    public class TimeRange
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public TimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;

            EnsureFieldsValidity();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="militaryStartTime">Ex: for 02:00, 04:10, 14:00 you should send 200, 410, 1400 respectively</param>
        /// <param name="militaryEndTime">Ex: for 02:00, 04:10, 14:00 you should send 200, 410, 1400 respectively</param>
        public TimeRange(short militaryStartTime, short militaryEndTime)
        {
            StartTime = ParseMilitaryTime(militaryStartTime);
            EndTime = ParseMilitaryTime(militaryEndTime);

            EnsureFieldsValidity();
        }

        public bool IsInTimeRange(DateTime date)
        {
            if (date.TimeOfDay >= StartTime && date.TimeOfDay <= EndTime)
                return true;
            else
                return false;
        }

        private TimeSpan ParseMilitaryTime(short time)
        {
            if (time < 0 || time >= 2400)
                throw new ArgumentOutOfRangeException("time", "Military times must be between 0 and 2359");

            if (time % 100 >= 60)
                throw new ArgumentOutOfRangeException("time", "Military times must be between 0 and 2359");

            return new TimeSpan((time / 100), (time % 100), 0);
        }

        private void EnsureFieldsValidity()
        {
            if (StartTime > EndTime)
                throw new InvalidOperationException("StartTime must be small than EndTime");
        }
    }
}
