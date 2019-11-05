using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Implementations
{
    public class ServiceBusTopic
    {
        private ITopicClient topicClient { get; }
        private ISubscriptionClient subscriptionClient { get; }
        private ConstructorCreateMode CurrentConstructorCreateMode { get; }

        /// <summary>
        /// Constructor identifier for validation
        /// </summary>
        private enum ConstructorCreateMode { SingleUse, WithTopic, OnlyConnectionString }

        /// <summary>
        /// Service bus connection string
        /// </summary>
        public string ServiceBusConnectionString { get; }

        /// <summary>
        /// Topic name
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Message received event handler
        /// </summary>
        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public ServiceBusTopic(string connectionString, string topicName, string subscriptionName)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(topicName) || string.IsNullOrEmpty(subscriptionName))
            {
                throw new ArgumentNullException();
            }
            CurrentConstructorCreateMode = ConstructorCreateMode.SingleUse;
            ServiceBusConnectionString = connectionString;
            EntityName = topicName;
            topicClient = new TopicClient(connectionString, topicName);
            subscriptionClient = new SubscriptionClient(connectionString, topicName, subscriptionName);
        }

        public ServiceBusTopic(string connectionString, string topicName)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(topicName))
            {
                throw new ArgumentNullException();
            }
            CurrentConstructorCreateMode = ConstructorCreateMode.WithTopic;
            ServiceBusConnectionString = connectionString;
            EntityName = topicName;
            topicClient = new TopicClient(connectionString, topicName);
        }

        public ServiceBusTopic(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }
            CurrentConstructorCreateMode = ConstructorCreateMode.OnlyConnectionString;
            ServiceBusConnectionString = connectionString;
        }

        /// <summary>
        /// Sends message to topic
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message, Dictionary<string, object> properties = null)
        {
            ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.SingleUse);
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    messageBody.UserProperties.Add(prop);
                }
            }
            await topicClient.SendAsync(messageBody);
        }

        /// <summary>
        /// Sends a service bus message to topic
        /// </summary>
        /// <param name="subscriptionName">Subscription to which the message has to be sent</param>
        /// <param name="message">Message to send</param>
        /// <returns></returns>
        public async Task SendMessageAsync(string subscriptionName, string message, Dictionary<string, object> properties = null)
        {
            ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.WithTopic);
            var subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, EntityName, subscriptionName);
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    messageBody.UserProperties.Add(prop);
                }
            }
            await topicClient.SendAsync(messageBody);
        }

        public async Task SendMessageAsync(string topicName, string subscriptionName, string message, Dictionary<string, object> properties = null)
        {
            ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.OnlyConnectionString);
            var topicClient = new TopicClient(ServiceBusConnectionString, topicName);
            var subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, EntityName, subscriptionName);
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    messageBody.UserProperties.Add(prop);
                }
            }
            await topicClient.SendAsync(messageBody);
        }

        #region Private Methods

        /// <summary>
        /// Registers the topic client for subscription
        /// </summary>
        private void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
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
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
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
            await subscriptionClient.AbandonAsync($"{ReceiveMode.PeekLock}");
            throw exceptionReceivedEventArgs.Exception;
        }

        private void ValidateTopicAndSubscriptionSettings(ConstructorCreateMode constructorCreateMode)
        {
            if (CurrentConstructorCreateMode != constructorCreateMode)
            {
                var argumentNull = new ArgumentNullException($"Undefined entity or subscription name");
                var invalidOperation = new InvalidOperationException($"Undefined entity or subscription name", argumentNull);
                throw invalidOperation;
            }
        }

        #endregion 
    }
}
