using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public interface IServiceBus
    {
        /// <summary>
        /// Connection string
        /// </summary>
        string ServiceBusConnectionString { get; }

        /// <summary>
        /// Entity name : Queue or topic
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// Method to send message via SB
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="numberOfMessagesToSend">Number of time the message to be sent</param>
        /// <returns>Task object</returns>
        Task SendMessage(string message);

        /// <summary>
        /// Method to get messages
        /// </summary>
        void GetMessage();

        /// <summary>
        /// Event raised after receiving message
        /// </summary>
        event EventHandler<MessageReceivedArgs> MessageReceived;
    }

    /// <summary>
    /// Event Arguments for Message received handler
    /// </summary>
    public class MessageReceivedArgs : EventArgs
    {
        public byte[] Data { get; }

        public MessageReceivedArgs(byte[] data)
        {
            Data = data;
        }
    }
}
