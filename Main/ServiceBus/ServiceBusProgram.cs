using ServiceBus;
using ServiceBus.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.ServiceBus
{
    public class ServiceBusProgram : IProgram
    {
        IServiceBus serviceBusQueue;
        IServiceBus serviceBusTopic;

        public ServiceBusProgram()
        {
            serviceBusQueue = new ServiceBusQueue(AppSettings.ServiceBusConnectionString, AppSettings.ServiceBusQueueName);
            serviceBusTopic = new ServiceBusTopic(AppSettings.ServiceBusConnectionString, AppSettings.ServiceBusTopicName, AppSettings.ServiceBusTopicSubscriptionName);
        }

        public void Run()
        {
            MainQueueAsync().GetAwaiter().GetResult();
            MainTopicAsync().GetAwaiter().GetResult();
        }

        async Task MainQueueAsync()
        {
            Console.WriteLine("Queue Program Starting");
            serviceBusQueue.MessageReceived += OnMessageReceived;
            await serviceBusQueue.SendMessage("Sample queue message");
            serviceBusQueue.GetMessage();
            Console.ReadKey();
            Console.WriteLine("Press any key to proceed");
        }

        async Task MainTopicAsync()
        {
            Console.WriteLine("Topic Program Starting");
            serviceBusTopic.MessageReceived += OnMessageReceived;
            await serviceBusTopic.SendMessage("Sample topic message");
            serviceBusTopic.GetMessage();
            Console.ReadKey();
            Console.WriteLine("Press any key to proceed");
        }

        private static void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            Console.WriteLine($"From main program \n Received message: Body:{Encoding.UTF8.GetString(e.Data)}");
        }
    }
}
