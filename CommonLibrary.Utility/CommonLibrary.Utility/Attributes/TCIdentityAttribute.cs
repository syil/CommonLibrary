using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Attributes
{
    public class TCIdentityAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string stringValue = value as string;

            if (stringValue != null)
            {
                long atcNo, btcNo, tcNo;
                long c1, c2, c3, c4, c5, c6, c7, c8, c9, q1, q2;

                tcNo = long.Parse(stringValue);

                atcNo = tcNo / 100;
                btcNo = tcNo / 100;

                c1 = atcNo % 10; atcNo = atcNo / 10;
                c2 = atcNo % 10; atcNo = atcNo / 10;
                c3 = atcNo % 10; atcNo = atcNo / 10;
                c4 = atcNo % 10; atcNo = atcNo / 10;
                c5 = atcNo % 10; atcNo = atcNo / 10;
                c6 = atcNo % 10; atcNo = atcNo / 10;
                c7 = atcNo % 10; atcNo = atcNo / 10;
                c8 = atcNo % 10; atcNo = atcNo / 10;
                c9 = atcNo % 10; atcNo = atcNo / 10;
                q1 = ((10 - ((((c1 + c3 + c5 + c7 + c9) * 3) + (c2 + c4 + c6 + c8)) % 10)) % 10);
                q2 = ((10 - (((((c2 + c4 + c6 + c8) + q1) * 3) + (c1 + c3 + c5 + c7 + c9)) % 10)) % 10);

                return ((btcNo * 100) + (q1 * 10) + q2 == tcNo);
            }
            else
            {
                return true;
            }
        }
    }
}
