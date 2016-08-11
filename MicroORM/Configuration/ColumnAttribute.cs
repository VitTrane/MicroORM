using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public ColumnAttribute(string dbColumnName) 
        {
            ColumnName = dbColumnName;
        }
    }
}
