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
        /// Message received event handler to be used with constructor #1
        /// </summary>
        public event EventHandler<MessageReceivedArgs> Subscription1;

        /// <summary>
        /// Message received event handler to be used with constructor #2
        /// </summary>
        public event EventHandler<MessageReceivedArgs> Subscription2;

        /// <summary>
        /// Message received event handler to be used with constructor #3
        /// </summary>
        public event EventHandler<MessageReceivedArgs> Subscription3;

        /// <summary>
        /// Construrctor #1
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topicName"></param>
        /// <param name="subscriptionName"></param>
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

        /// <summary>
        /// Construrctor #2
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topicName"></param>
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

        /// <summary>
        /// Construrctor #3
        /// </summary>
        /// <param name="connectionString"></param>
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

        public async Task RegisterMessageReceiverHandler()
        {
            await Task.Run(() => 
            {
                ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.SingleUse);
                RegisterMessageHandler(subscriptionClient, ConstructorCreateMode.SingleUse);
            });
        }

        public async Task RegisterMessageReceiverHandler(string subscriptionName)
        {
            await Task.Run(() =>
            {
                ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.WithTopic);
                RegisterMessageHandler(subscriptionClient, ConstructorCreateMode.WithTopic);
            });
        }

        public async Task RegisterMessageReceiverHandler(string topicName, string subscriptionName)
        {
            await Task.Run(() =>
            {
                ValidateTopicAndSubscriptionSettings(ConstructorCreateMode.OnlyConnectionString);
                RegisterMessageHandler(subscriptionClient, ConstructorCreateMode.OnlyConnectionString);
            });
        }

        #region Private Methods

        /// <summary>
        /// Registers the topic client for subscription
        /// </summary>
        private void RegisterMessageHandler(ISubscriptionClient subscriptionClient, ConstructorCreateMode constructorCreateMode)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            switch (constructorCreateMode)
            {
                case ConstructorCreateMode.SingleUse:
                    subscriptionClient.RegisterMessageHandler(RegisterSubscription1, messageHandlerOptions);
                    break;
                case ConstructorCreateMode.WithTopic:
                    subscriptionClient.RegisterMessageHandler(RegisterSubscription2, messageHandlerOptions);
                    break;
                case ConstructorCreateMode.OnlyConnectionString:
                    subscriptionClient.RegisterMessageHandler(RegisterSubscription3, messageHandlerOptions);
                    break;
                default:
                    throw new InvalidOperationException("Invalid constructor mode");
            }
        }

        /// <summary>
        /// Receives message and invokes the message received handler
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task RegisterSubscription1(Message message, CancellationToken token)
        {
            Subscription1?.Invoke(this, new MessageReceivedArgs(message.Body));
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        /// <summary>
        /// Receives message and invokes the message received handler
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task RegisterSubscription2(Message message, CancellationToken token)
        {
            Subscription2?.Invoke(this, new MessageReceivedArgs(message.Body));
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        /// <summary>
        /// Receives message and invokes the message received handler
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task RegisterSubscription3(Message message, CancellationToken token)
        {
            Subscription3?.Invoke(this, new MessageReceivedArgs(message.Body));
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
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

        /// <summary>
        /// Validate settings based on constructor used
        /// </summary>
        /// <param name="constructorCreateMode">Constructor mode to validate</param>
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
