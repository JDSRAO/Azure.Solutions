using CosmosDB.MongoDB.Driver;
using Main.CosmosDB.Mongo.Driver.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.CosmosDB.Mongo.Driver
{
    class MongoDriverSingleInstanceProgram : IProgram
    {
        public ILogger Logger { get; set; }

        private MongoDriverSingleInstance mongoDriver;
        private const string connectionString = AppSettings.CosmosDB_MongoDBConnectionString;
        private const string database = AppSettings.CosmosDB_MongoDBDatabase;
        private const string collection = AppSettings.CosmosDB_MongoDBCollection;

        public MongoDriverSingleInstanceProgram()
        {
            mongoDriver = new MongoDriverSingleInstance(connectionString, database);
        }

        public void Run()
        {
            Console.WriteLine("Starting Mongo DB driver program");
            ShowDBS().GetAwaiter().GetResult();
            DropCollection().GetAwaiter().GetResult();
            InsertDocument().GetAwaiter().GetResult();
            GetAllDocuments().GetAwaiter().GetResult();
            DropDatabase().GetAwaiter().GetResult();
            Console.WriteLine("Press any key to proceed");
            Console.ReadKey();
        }

        private async Task ShowDBS()
        {
            try
            {
                var dbs = await mongoDriver.GetAllDatabasesAsync();
                Console.Write($"Databases: ");
                foreach (var db in dbs)
                {
                    Console.Write($"{db}, ");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async Task DropCollection()
        {
            try
            {
                await mongoDriver.DropCollectionAsync(collection);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async Task InsertDocument()
        {
            Console.WriteLine($"Inserting data into collection: {collection}");
            var employee = new Employee()
            {
                ID = 31,
                Name = "K. J. D. S. Srinivasa Rao",
                Email = "Srinivas@cloudthing.com"
            };
            await mongoDriver.InsertData<Employee>(collection, employee);
        }

        private async Task GetAllDocuments()
        {
            Console.WriteLine($"Getting all documents from collecton: {collection}");
            var employees = await mongoDriver.FindAllDocuments<Employee>(collection);
            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.ToString()}");
            }

        }

        private async Task DropDatabase()
        {
            try
            {
                Console.WriteLine($"Press any key to drop database: {database}");
                Console.ReadLine();
                await mongoDriver.DropDatabaseAsync();
                Console.WriteLine("Deleted successfully");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
