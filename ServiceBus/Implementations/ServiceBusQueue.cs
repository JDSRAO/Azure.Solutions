using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Implementations
{
    public class ServiceBusQueue : IServiceBus
    {
        private static IQueueClient queueClient;

        public string ServiceBusConnectionString { get; }

        public string EntityName { get; }

        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public ServiceBusQueue(string connectionString, string queueName)
        {
            ServiceBusConnectionString = connectionString;
            EntityName = queueName;
            queueClient = new QueueClient(connectionString, queueName);
        }

        public void GetMessage()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        public async Task SendMessage(string message)
        {
            try
            {
                var messageBody = new Message(Encoding.UTF8.GetBytes(message));
                await queueClient.SendAsync(messageBody);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #region Private Methods

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                MessageReceived?.Invoke(this, new MessageReceivedArgs(message.Body));
                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            await queueClient.AbandonAsync($"{ReceiveMode.PeekLock}");
            //return Task.CompletedTask;
        }

        #endregion
    }
}
