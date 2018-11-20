using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Implementations
{
    public class ServiceBusTopic : IServiceBus
    {
        private static ITopicClient topicClient;
        private static ISubscriptionClient subscriptionClient;

        public string ServiceBusConnectionString { get; }

        public string EntityName { get; }

        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public ServiceBusTopic(string connectionString, string topicName, string subscriptionName)
        {
            ServiceBusConnectionString = connectionString;
            EntityName = topicName;
            topicClient = new TopicClient(connectionString, topicName);
            subscriptionClient = new SubscriptionClient(connectionString, topicName, subscriptionName);
        }

        public void GetMessage()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        public async Task SendMessage(string message)
        {
            try
            {
                var messageBody = new Message(Encoding.UTF8.GetBytes(message));
                await topicClient.SendAsync(messageBody);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        #region Private Methods

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                MessageReceived?.Invoke(this, new MessageReceivedArgs(message.Body));
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            await subscriptionClient.AbandonAsync($"{ReceiveMode.PeekLock}");
            //return Task.CompletedTask;
        }

        #endregion 
    }
}
