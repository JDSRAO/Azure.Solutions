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

        public BlobProgram()
        {
            blob = new Blob(AppSettings.StorageAccountConnectionString);
        }

        public void Run()
        {
            Console.WriteLine("Blob Storage program starting");
            CreateBlobContainerAsync().GetAwaiter().GetResult();
            UploadFromFileAsync().GetAwaiter().GetResult();
            DownloadFromFileAsync().GetAwaiter().GetResult();
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


    }
}
