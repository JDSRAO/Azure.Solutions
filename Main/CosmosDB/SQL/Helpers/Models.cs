using System;
using System.Collections.Generic;
using System.Text;

namespace Main.CosmosDB.SQL.Helpers
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Department Department { get; set; }

        public override string ToString()
        {
            return $"{ID}, {Name}, {Email}";
        }
    }

    public class Department
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
