using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        static BaseRepository<Customer, int> rep = new BaseRepository<Customer, int>("System.Data.SQLite.EF6", "Data Source=TestDb.db;");
        static void Main(string[] args)
        {            
            Customer cust = rep.GetById(1);
            List<Customer> c = rep.SelectWhere(x => x.LastName == "Петрова").ToList();
            cust.Name ="UpdateName";
            rep.Save(cust);

            Console.ReadLine();
        }
    }
}
