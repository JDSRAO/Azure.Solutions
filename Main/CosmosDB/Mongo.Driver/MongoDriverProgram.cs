using CosmosDB.MongoDB.Driver;
using Main.CosmosDB.Mongo.Driver.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.CosmosDB.Mongo.Driver
{
    public class MongoDriverProgram : IProgram
    {
        private MongoDriver mongoDriver;
        private const string connectionString = AppSettings.CosmosDB_MongoDBConnectionString;
        private const string database = AppSettings.CosmosDB_MongoDBDatabase;
        private const string collection = AppSettings.CosmosDB_MongoDBCollection;

        public ILogger Logger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MongoDriverProgram()
        {
            mongoDriver = new MongoDriver(connectionString);
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
                throw ex;
            }
        }

        private async Task DropCollection()
        {
            try
            {
                await mongoDriver.DropCollectionAsync(database, collection);
            }
            catch (Exception ex)
            {
                throw ex;
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
            await mongoDriver.InsertData<Employee>(database, collection, employee);
        }

        private async Task GetAllDocuments()
        {
            Console.WriteLine($"Getting all documents from collecton: {collection}");
            var employees = await mongoDriver.FindAllDocuments<Employee>(database, collection);
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
                await mongoDriver.DropDatabaseAsync(database);
                Console.WriteLine("Deleted successfully");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
