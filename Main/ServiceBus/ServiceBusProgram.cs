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
        ServiceBusQueue serviceBusQueue;
        ServiceBusTopic serviceBusTopic;

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
            Console.WriteLine("Queue Program Started");
            try
            {
                serviceBusQueue.MessageReceived += OnMessageReceived;
                Console.WriteLine("Sending message to service bus");
                await serviceBusQueue.SendMessageAsync("Sample queue message");
                Console.WriteLine("Message sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred" + ex.Message);
            }
            
            Console.WriteLine("Press any key to proceed");
            Console.ReadKey();
        }

        async Task MainTopicAsync()
        {
            Console.WriteLine("Topic Program Started");
            try
            {
                serviceBusTopic.Subscription1 += OnMessageReceived;
                Console.WriteLine("Sending message to service bus");
                await serviceBusTopic.SendMessageAsync("Sample topic message");
                Console.WriteLine("Message sent successfully");
                var serviceBusClient = new ServiceBusTopic(AppSettings.ServiceBusConnectionString, AppSettings.ServiceBusTopicName); //, 
                                                                                                                                     //await serviceBusTopic.SendMessageAsync("Sample topic message");
                await serviceBusClient.SendMessageAsync($"{AppSettings.ServiceBusTopicSubscriptionName}1", "Sample topic message");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred" + ex.Message);
            }
            
            Console.WriteLine("Press any key to proceed");
            Console.ReadKey();
        }

        private static void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            Console.WriteLine($"From main program \n Received message: Body:{Encoding.UTF8.GetString(e.Data)}");
        }
    }
}
