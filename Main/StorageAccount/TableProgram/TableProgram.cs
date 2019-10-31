using Main.StorageAccount.TableProgram.Entities;
using StorageAccount.TableStorage;
using StorageAccount.TableStorage.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.StorageAccount.TableProgram
{
    public class TableProgram : IProgram
    {
        private Table table;
        private const string tableName = AppSettings.StorageAccountTableName;

        public ILogger Logger { get; set; }

        public TableProgram()
        {
            table = new Table(AppSettings.StorageAccountConnectionString);
        }

        public void Run()
        {
            CreateTableAsync().GetAwaiter().GetResult();
            InsertDataAsync().GetAwaiter().GetResult();
            InsertElasticDataAsync().GetAwaiter().GetResult();
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

        private async Task InsertElasticDataAsync()
        {
            try
            {
                var transactionID = Guid.NewGuid().ToString();
                var elasticTableName = "ElasticData";
                await table.CreateTableAsync(elasticTableName);
                Console.WriteLine($"Inserting data into table: {elasticTableName}");
                List<ElasticTableEntity> items = new List<ElasticTableEntity>();
                for (int i = 0; i < 10; i++)
                {
                    dynamic item = new ElasticTableEntity();
                    item.PartitionKey = transactionID;
                    item.RowKey = Guid.NewGuid().ToString();
                    for (int j = 0; j < 20; j++)
                    {
                        item[$"property{j}"] = $"value{j}";
                    }
                    items.Add(item);
                }

                await table.InsertBulkElasticAsync(elasticTableName, items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertElasticDataAsync : {ex.Message}");
            }
            
        }

        public static string ToTimeStamp(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmssffff");
        }
    }
}
