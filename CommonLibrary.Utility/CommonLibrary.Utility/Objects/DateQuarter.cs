using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace CommonLibrary.Objects
{
    [Serializable]
    public struct DateQuarter
    {
        private int quarter;
        private int year;

        public int QuarterPart { get { return quarter; } }
        public int Year { get { return year; } }

        public DateQuarter(DateTime date)
        {
            this.quarter = (date.Month + 2) / 3;
            this.year = date.Year;
        }

        [JsonConstructor]
        public DateQuarter(int quarterPart, int year)
        {
            this.quarter = quarterPart;
            this.year = year;
        }

        public DateQuarter AddYear(int year)
        {
            return new DateQuarter(this.quarter, this.year + year);
        }

        public DateQuarter AddQuarter(int quarter)
        {
            int newQuarter = this.quarter;
            int newYear = this.Year;

            int sign = Math.Sign(quarter);

            for (int i = 0; i < Math.Abs(quarter); i++)
            {
                newQuarter += sign;

                if (newQuarter < 1)
                {
                    newQuarter = 4;
                    newYear -= 1;
                }
                else if (newQuarter > 4)
                {
                    newQuarter = 1;
                    newYear += 1;
                }
            }

            return new DateQuarter(newQuarter, newYear);
        }

        public DateQuarter PreviousQuarter()
        {
            return AddQuarter(-1);
        }

        public DateQuarter NextQuarter()
        {
            return AddQuarter(1);
        }

        public override string ToString()
        {
            return string.Format("{0}Ç{1}", this.quarter, this.year);
        }
    }
}
