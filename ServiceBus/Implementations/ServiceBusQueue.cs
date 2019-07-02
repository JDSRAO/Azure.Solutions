﻿using Microsoft.Azure.ServiceBus;
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

        public ServiceBusQueue(string connectionString, string queueName)
        {
            if(string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException();
            }
            ServiceBusConnectionString = connectionString;
            EntityName = queueName;
            queueClient = new QueueClient(connectionString, queueName);
            RegisterMessageHandler();
        }

        /// <summary>
        /// Send message to queue
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="useSessions"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message, bool useSessions = false)
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

        #endregion
    }
}
