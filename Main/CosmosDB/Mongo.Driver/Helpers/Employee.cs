using CosmosDB.MongoDB.Driver.Bson.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main.CosmosDB.Mongo.Driver.Helpers
{
    public class Employee
    {
        [BsonId]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [BsonIgnore]
        public string Department { get; set; }

        public override string ToString()
        {
            return $"{ID}, {Name}, {Email}";
        }
    }
}
