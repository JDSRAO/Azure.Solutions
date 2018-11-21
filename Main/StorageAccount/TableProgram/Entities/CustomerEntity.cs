using StorageAccount.TableStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main.StorageAccount.TableProgram.Entities
{
    public class CustomerEntity : BaseEntity
    {
        public override string TableName => "customerData";
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public CustomerEntity()
        {

        }

        public CustomerEntity(string department)
        {
            PartitionKey = department;
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
