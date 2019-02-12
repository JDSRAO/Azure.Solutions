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
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(topicName) || string.IsNullOrEmpty(subscriptionName))
            {
                throw new ArgumentNullException();
            }
            ServiceBusConnectionString = connectionString;
            EntityName = topicName;
            topicClient = new TopicClient(connectionString, topicName);
            subscriptionClient = new SubscriptionClient(connectionString, topicName, subscriptionName);
            RegisterMessageHandler();
        }

        public async Task SendMessageAsync(string message, bool useSessions = false)
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

        private void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

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
            await subscriptionClient.AbandonAsync($"{ReceiveMode.PeekLock}");
            throw exceptionReceivedEventArgs.Exception;
        }

        #endregion 
    }
}
