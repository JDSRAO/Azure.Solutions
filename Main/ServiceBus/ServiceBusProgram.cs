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

        public ILogger Logger { get; set; }

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
            await serviceBusQueue.SendMessageAsync("Sample queue message");
            serviceBusQueue.GetMessageAsync();
            Console.ReadKey();
            Console.WriteLine("Press any key to proceed");
        }

        async Task MainTopicAsync()
        {
            Console.WriteLine("Topic Program Starting");
            serviceBusTopic.MessageReceived += OnMessageReceived;
            await serviceBusTopic.SendMessageAsync("Sample topic message");
            serviceBusTopic.GetMessageAsync();
            Console.ReadKey();
            Console.WriteLine("Press any key to proceed");
        }

        private static void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            Console.WriteLine($"From main program \n Received message: Body:{Encoding.UTF8.GetString(e.Data)}");
        }
    }
}
