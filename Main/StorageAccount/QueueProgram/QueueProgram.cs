using StorageAccount.QueueStorage;
using System;
//using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Main.StorageAccount.QueueProgram
{
    public class QueueProgram : IProgram
    {
        private Queue queue;
        private const string queueName = AppSettings.StorageAccountQueueame;

        public ILogger Logger { get; set; }

        public QueueProgram()
        {
            queue = new Queue(AppSettings.StorageAccountConnectionString);
        }

        public void Run()
        {
            CreateQueueAsync().GetAwaiter().GetResult();
            EnQueueAsync().GetAwaiter().GetResult();
        }

        private async Task EnQueueAsync()
        {
            await queue.EnqueueAsync(queueName, "sample hello world testing message");
        }

        private async Task CreateQueueAsync()
        {
            await queue.CreateQueueAsync(queueName);
        }
    }
}
