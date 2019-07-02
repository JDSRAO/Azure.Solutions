using Main.CosmosDB.Mongo.Driver;
using Main.CosmosDB.SQL;
using Main.RedisCache;
using Main.ServiceBus;
using Main.StorageAccount.BlobProgram;
using Main.StorageAccount.QueueProgram;
using Main.StorageAccount.TableProgram;
using System;
using System.Text;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Application starting");
            builder.AppendLine("1. Service Bus Application");
            builder.AppendLine("2. Redis Cache");
            builder.AppendLine("3. Function app");
            builder.AppendLine("4. Blob Storage Account");
            builder.AppendLine("5. Table Storage Account");
            builder.AppendLine("6. Queue Storage Account");
            builder.AppendLine("7. Cosmos DB : MongoDB Driver");
            builder.AppendLine("8. Cosmos DB : MongoDB Driver Single Instance");
            builder.AppendLine("9. Cosmos DB : SQL API");
            builder.AppendLine("Enter your choice: ");
            bool run = true;
            var logger = new FileLogger("logger.txt");
            while (run)
            {
                Console.Clear();
                IProgram program;
                Console.Write(builder.ToString());
                int choice = 0;
                int.TryParse(Console.ReadLine(), out choice);

                switch (choice)
                {
                    case 1:
                        program = new ServiceBusProgram();
                        break;
                    case 2:
                        program = new RedisProgram();
                        break;
                    case 3:
                        program = new BlobProgram();
                        break;
                    case 4:
                        program = new BlobProgram();
                        break;
                    case 5:
                        program = new TableProgram();
                        break;
                    case 6:
                        program = new QueueProgram();
                        break;
                    case 7:
                        program = new MongoDriverProgram();
                        break;
                    case 8:
                        program = new MongoDriverSingleInstanceProgram();
                        break;
                    case 9:
                        program = new SQLProgram();
                        break;
                    default:
                        program = null;
                        Environment.Exit(-1);
                        break;
                }
                program.Logger = logger;
                program.Run();
            }
            Console.ReadKey();
        }
    }
}
