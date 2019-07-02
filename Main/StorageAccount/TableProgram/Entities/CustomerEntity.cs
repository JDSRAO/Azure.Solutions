using StorageAccount.TableStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main.StorageAccount.TableProgram.Entities
{
    public class CustomerEntity : BaseEntity
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public CustomerEntity() : base("customerData")
        {

        }

        public CustomerEntity(string department) : base("customerData")
        {
            PartitionKey = department;
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
