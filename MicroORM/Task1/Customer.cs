using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    [Table("Customer")]
    public class Customer
    {
        [PrimaryKey]
        [Column("Id")]
        public int Id { get;  set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }
    }
}
