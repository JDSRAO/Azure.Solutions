using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Implementations
{
    public class ServiceBusQueue
    {
        private IQueueClient queueClient { get; }
        private ConstructorCreateMode CurrentConstructorCreateMode { get; }

        /// <summary>
        /// Constructor identifier for validation
        /// </summary>
        private enum ConstructorCreateMode { WithQueue, OnlyConnectionString }

        /// <summary>
        /// Service bus queue connection string
        /// </summary>
        public string ServiceBusConnectionString { get; }

        /// <summary>
        /// Queue name
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Message received event handler
        /// </summary>
        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public ServiceBusQueue(string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Connection string is empty or null");
            }
            ServiceBusConnectionString = connectionString;
            CurrentConstructorCreateMode = ConstructorCreateMode.OnlyConnectionString;
        }

        public ServiceBusQueue(string connectionString, string queueName)
        {
            if(string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("Connection string or queue name is either null or empty");
            }
            CurrentConstructorCreateMode = ConstructorCreateMode.WithQueue;
            ServiceBusConnectionString = connectionString;
            EntityName = queueName;
            queueClient = new QueueClient(connectionString, queueName);
        }

        /// <summary>
        /// Sends message to queue
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.WithQueue);
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            await queueClient.SendAsync(messageBody);
        }

        /// <summary>
        /// Sends message to queue
        /// </summary>
        /// <param name="queueName">Queue to which the message has to be sent</param>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string queueName, string message)
        {
            ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.OnlyConnectionString);
            var queueClient = new QueueClient(ServiceBusConnectionString, queueName);
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            await queueClient.SendAsync(messageBody);
        }

        #region Private Methods

        /// <summary>
        /// Registers the queue client for subscription
        /// </summary>
        private void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        /// <summary>
        /// Receives message and invokes the message received handler
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Excetion handler 
        /// </summary>
        /// <param name="exceptionReceivedEventArgs"></param>
        /// <returns></returns>
        private async Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            await queueClient.AbandonAsync($"{ReceiveMode.PeekLock}");
            throw exceptionReceivedEventArgs.Exception;
        }

        private void ValidateTopicAndSubscriptionSettings(ConstructorCreateMode constructorCreateMode)
        {
            if (CurrentConstructorCreateMode != constructorCreateMode)
            {
                var argumentNull = new ArgumentNullException($"Undefined queue name");
                var invalidOperation = new InvalidOperationException($"Undefined queue name", argumentNull);
                throw invalidOperation;
            }
        }

        #endregion
    }
}
