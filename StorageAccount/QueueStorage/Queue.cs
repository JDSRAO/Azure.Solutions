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

        public async Task EnqueueAsync(string queueName, string content)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                var message = new CloudQueueMessage(content);
                await queue.AddMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> PeekAsync(string queueName)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                var message = await queue.GetMessageAsync();
                return message.AsString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateMessageAsync(string queueName, string updatedContent)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                var message = await queue.GetMessageAsync();
                message.SetMessageContent(updatedContent);
                await queue.UpdateMessageAsync(message, TimeSpan.FromSeconds(10), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DequeueAsync(string queueName)
        {
            try
            {
                var queue = queueClient.GetQueueReference(queueName);
                var message = await queue.GetMessageAsync();
                Console.WriteLine($"received message {message.AsString}");
                await queue.DeleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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


    }
}
