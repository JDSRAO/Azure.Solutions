using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageAccount.QueueStorage
{
    public class Queue
    {
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;

        /// <summary>
        /// Queue storage connection string
        /// </summary>
        public string ConnectionString { get; }

        public Queue(string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("No connection string found");
            }
            else if(CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                ConnectionString = connectionString;
                queueClient = storageAccount.CreateCloudQueueClient();
            }
        }

        /// <summary>
        /// Creates queue
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <returns>Creation status</returns>
        public async Task<bool> CreateQueueAsync(string queueName)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                return await queue.CreateIfNotExistsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds a message into queue
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <param name="content">Message to add</param>
        public async Task EnqueueAsync(string queueName, string content)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                await CheckIfQueueExists(queueName, queue);
                var message = new CloudQueueMessage(content);
                await queue.AddMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets a message from queue without deleting it from queue
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <returns>Message</returns>
        public async Task<string> PeekAsync(string queueName)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                await CheckIfQueueExists(queueName, queue);
                var message = await queue.GetMessageAsync();
                return message.AsString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates a message in queue
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <param name="updatedContent">Content to update</param>
        public async Task UpdateMessageAsync(string queueName, string updatedContent)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                await CheckIfQueueExists(queueName, queue);
                var message = await queue.GetMessageAsync();
                message.SetMessageContent(updatedContent);
                await queue.UpdateMessageAsync(message, TimeSpan.FromSeconds(10), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets a message from queue and deletes it
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <returns>Message</returns>
        public async Task<string> DequeueAsync(string queueName)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                await CheckIfQueueExists(queueName, queue);
                var message = await queue.GetMessageAsync();
                await queue.DeleteMessageAsync(message);
                return message.AsString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete queue
        /// </summary>
        /// <param name="queueName">Queue name</param>
        /// <returns>Deletion status</returns>
        public async Task<bool> DeleteQueueAsync(string queueName)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                return await queue.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task CheckIfQueueExists(string queueName, CloudQueue queue)
        {
            var exists = await queue.ExistsAsync();
            if (!exists)
            {
                throw new Exception($"No queue with {queueName} exists");
            }
        }


    }
}
