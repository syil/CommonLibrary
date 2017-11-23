using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnNameAttribute : Attribute
    {
        private readonly string columnName;

        public ColumnNameAttribute(string columnName)
        {
            this.columnName = columnName;
        }

        public string ColumnName
        {
            get { return columnName; }
        }
    }
}
