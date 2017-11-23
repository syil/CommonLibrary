using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class StringValueAttribute : Attribute
    {
        private readonly string stringValue;

        public StringValueAttribute(string stringValue)
        {
            this.stringValue = stringValue;
        }

        public string StringValue
        {
            get { return stringValue; }
        }
    }
}
