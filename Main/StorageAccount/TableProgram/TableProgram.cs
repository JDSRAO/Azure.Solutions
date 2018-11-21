using Main.StorageAccount.TableProgram.Entities;
using StorageAccount.TableStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.StorageAccount.TableProgram
{
    public class TableProgram : IProgram
    {
        private Table table;
        private const string tableName = "customerData";

        public TableProgram()
        {
            table = new Table(AppSettings.StorageAccountConnectionString);
        }

        public void Run()
        {
            CreateTableAsync().GetAwaiter().GetResult();
            InsertDataAsync().GetAwaiter().GetResult();
            Console.WriteLine("Press any key to proceed");
            Console.ReadKey();
        }

        private async Task CreateTableAsync()
        {
            Console.WriteLine($"Creating table : {tableName}");
            await table.CreateTableAsync(tableName);
        }

        private async Task InsertDataAsync()
        {
            Console.WriteLine($"Inserting data into: {tableName}");
            var customer = new CustomerEntity("Sales")
            {
                Email = "sales@cloudthing.com",
                PhoneNumber = "+91 74065 36699"
            };

            await table.InsertAsync(customer);
        }
    }
}
