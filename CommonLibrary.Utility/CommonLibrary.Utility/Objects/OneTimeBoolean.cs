using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Objects
{
    public class OneTimeBoolean
    {
        private bool innerValue;

        public bool HasChanged { get; private set; }

        public bool Value
        {
            get
            {
                if (!HasChanged)
                {
                    HasChanged = true;
                    return innerValue;
                }
                else
                {
                    return !innerValue;
                }
            }
        }

        public OneTimeBoolean(bool b)
        {
            this.innerValue = b;
        }

        public static implicit operator bool(OneTimeBoolean otb)
        {
            return otb.Value;
        }

        public static implicit operator OneTimeBoolean(bool b)
        {
            return new OneTimeBoolean(b);
        }

        public override string ToString()
        {
            return string.Format("Initial Value: [{0}], Changed: [{1}]", innerValue, HasChanged);
        }
    }
}
