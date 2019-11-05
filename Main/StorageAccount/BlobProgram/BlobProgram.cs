using StorageAccount.BlobStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Main.StorageAccount.BlobProgram
{
    public class BlobProgram : IProgram
    {
        private Blob blob;
        private const string containerName = AppSettings.StorageAccountBlobContainerName;
        private const string blobName = AppSettings.StorageAccountBlobName;

        public ILogger Logger { get; set; }

        public BlobProgram()
        {
            blob = new Blob(AppSettings.StorageAccountConnectionString);
        }

        public void Run()
        {
            Console.WriteLine("Blob Storage program starting");
            CreateBlobContainerAsync().GetAwaiter().GetResult();
            UploadFromFileAsync().GetAwaiter().GetResult();
            //DownloadFromFileAsync().GetAwaiter().GetResult();
            //ReadAsStreamAsync().GetAwaiter().GetResult();
            UploadTextAsync().GetAwaiter().GetResult();
            Console.WriteLine("Press any key to proceed");
            Console.ReadKey();
        }

        private async Task CreateBlobContainerAsync()
        {
            Console.WriteLine("Creating blob container");
            await blob.CreateBlobContainerIfnotExistsAsync(containerName);
            Console.WriteLine("Container created successfully");
        }

        private async Task UploadFromFileAsync()
        {
            Console.WriteLine("Uploading file to blob");
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
            var sourceFile = Path.Combine(localPath, localFileName);
            File.WriteAllText(sourceFile, "Hello, World!");
            await blob.UploadFromFileAsync(containerName, blobName, sourceFile);
            Console.WriteLine("Upload successfull");
        }

        private async Task DownloadFromFileAsync()
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
            var sourceFile = Path.Combine(localPath, localFileName);
            File.WriteAllText(sourceFile, "Hello, World!");
            await blob.DownloadToFileAsync(containerName, blobName, sourceFile);
        }

        private async Task ReadAsStreamAsync()
        {
            var contentStream = await blob.ReadAsStreamAsync(containerName, blobName);
            using (var streamReader = new StreamReader(contentStream))
            {
                var content = await streamReader.ReadToEndAsync();
                Console.WriteLine($"The content of the blob is :");
                Console.WriteLine($"{content}");
            }
        }

        private async Task UploadTextAsync()
        {
            var content = @"Region,Country,Item Type,Sales Channel,Order Priority,Order Date,Order ID,Ship Date,Units Sold,Unit Price,Unit Cost,Total Revenue,Total Cost,Total Profit
Europe,Latvia,Beverages,Online,C,12 / 28 / 2015,361825549,1 / 23 / 2016,1075,47.45,31.79,51008.75,34174.25,16834.5
Middle East and North Africa,Pakistan,Vegetables,Offline,C,1 / 13 / 2011,141515767,02 / 01 / 2011,6515,154.06,90.93,1003700.9,592408.95,411291.95
";
            var contentType = "text/csv";
            await blob.UploadTextAsync(containerName, "blobName.csv", content, contentType);
        }

    }
}
