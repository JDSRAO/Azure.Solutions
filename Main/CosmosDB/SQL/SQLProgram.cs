using CosmosDB.SQL;
using Main.CosmosDB.SQL.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.CosmosDB.SQL
{
    public class SQLProgram : IProgram
    {
        private const string connectionString = AppSettings.CosmosDB_SQLConnectionString;
        private const string key = AppSettings.ComsomDB_SQLKey;
        private const string databaseId = AppSettings.CosmosDB_SQLDatabaseId;
        private string collectionId = "Employee";
        private SQLDriver driver;

        public SQLProgram()
        {
            driver = new SQLDriver(connectionString, key, databaseId);
            driver.CollectionId = collectionId;
        }


        public void Run()
        {
            Console.WriteLine("Starting Cosmos DB driver program");
            CreateDatabaseAsync().GetAwaiter().GetResult();
            CreateCollectionAsync().GetAwaiter().GetResult();
            PrintAllCollections().GetAwaiter().GetResult();
            //InsertDocumentIntoCollection().GetAwaiter().GetResult();
            PrintAllDocuments().GetAwaiter().GetResult();
            Console.WriteLine("Press any key to proceed");
            Console.ReadLine();
        }

        private async Task CreateDatabaseAsync()
        {
            try
            {
                Console.WriteLine($"Creating Cosmos DB : SQL API Database: {databaseId}");
                await driver.CreateDatabseIfNotExistsAsync();
                Console.WriteLine("Database created successfully");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task CreateCollectionAsync()
        {
            try
            {
                Console.WriteLine($"Creating collection : {collectionId}");
                await driver.CreateCollectionIfNotExistsAsync(collectionId);
                Console.WriteLine("Created successfully");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task PrintAllCollections()
        {
            Console.WriteLine($"Getting all collections from database: {databaseId}");
            var collections = await driver.GetAllCollections();
            foreach (var collection in collections)
            {
                Console.WriteLine($"\t {collection}");
            }
        }

        private async Task InsertDocumentIntoCollection()
        {
            Console.WriteLine($"Inserting into collection {collectionId} ");
            var employee = new Employee
            {
                ID = 1,
                Name = "KJDS Srinivasa Rao",
                Email = "srinivas@cloudthing.com",
                Department = new Department
                {
                    ID = 1,
                    Name = "IT",
                    Email = "it@cloudthing.com"
                }
            };
            await driver.InsertDocumentAsync(employee, collectionId);
            Console.WriteLine("Inserted successfully");
        }

        private async Task PrintAllDocuments()
        {
            try
            {
                Console.WriteLine($"Getting all documents from {collectionId}");
                var documents = await driver.GetAllDocumentsAsync<Employee>();
                foreach (var document in documents)
                {
                    Console.WriteLine($"\t {document}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
