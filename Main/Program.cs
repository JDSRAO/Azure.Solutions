using Main.RedisCache;
using Main.ServiceBus;
using Main.StorageAccount.BlobProgram;
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
            builder.AppendLine("Enter your choice: ");
            //builder.AppendLine("3. ");
            bool run = true;
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
                    default:
                        program = null;
                        Environment.Exit(-1);
                        break;
                }

                program.Run();
            }
            Console.ReadKey();
        }
    }
}
