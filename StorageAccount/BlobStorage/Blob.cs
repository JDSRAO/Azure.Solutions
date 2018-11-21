using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageAccount.BlobStorage
{
    public class Blob
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;

        public string ConnectionString { get; }

        public Blob(string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("No connection string found");
            }
            else if(CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                ConnectionString = connectionString;
                blobClient = storageAccount.CreateCloudBlobClient();
            }
            else
            {
                throw new Exception("A connection string has not been defined in the system environment variables. Add a environment variable named 'storageconnectionstring' with your storage connection string as a value");
            }
        }

        public async Task<bool> CreateBlobContainerIfnotExistsAsync(string blobName)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(blobName);
                
                var status = await blobContainer.CreateIfNotExistsAsync();
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await blobContainer.SetPermissionsAsync(permissions);
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UploadFromFileAsync(string containerName, string blobName, string path)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                var blockBlob = blobContainer.GetBlockBlobReference(blobName);
                await blockBlob.UploadFromFileAsync(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Uri>> GetAllBlobsAsync(string containerName)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                List<Uri> blobs = new List<Uri>();

                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await blobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    blobContinuationToken = results.ContinuationToken;
                    blobs.AddRange(results.Results.Select(x => x.Uri));
                    return blobs;
                }
                while (blobContinuationToken != null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DownloadToFileAsync(string containerName, string blobName, string path, FileMode fileMode = FileMode.Create)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                var blockBlob = blobContainer.GetBlobReference(blobName);
                await blockBlob.DownloadToFileAsync(path, fileMode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteBlobContainerAsync(string containerName)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                await blobContainer.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
