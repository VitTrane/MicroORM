using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; private set; }

        public TableAttribute(string dbTableName) 
        {
            TableName = dbTableName;
        }
    }
}
