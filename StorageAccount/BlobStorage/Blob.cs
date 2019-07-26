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

        /// <summary>
        /// Blob storage connection string
        /// </summary>
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

        /// <summary>
        /// Creates a blob container
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>True if created successfully else false</returns>
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

        /// <summary>
        /// Upload a file to blob storage
        /// </summary>
        /// <param name="containerName">Blob container name</param>
        /// <param name="blobName">Blob name</param>
        /// <param name="path">Path of the file to upload</param>
        /// <returns>Task object</returns>
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


        public async Task UploadFromStreamAsync(string containerName, string blobName, Stream source)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                var blockBlob = blobContainer.GetBlockBlobReference(blobName);
                await blockBlob.UploadFromStreamAsync(source);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Upload a file to blob storage
        /// </summary>
        /// <param name="containerName">Blob container name</param>
        /// <param name="blobName">Blob name</param>
        /// <param name="content">Content to upload</param>
        /// <param name="contentType">Content type being uploaded</param>
        /// <returns></returns>
        public async Task UploadTextAsync(string containerName, string blobName, string content, string contentType)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                var blockBlob = blobContainer.GetBlockBlobReference(blobName);
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.UploadTextAsync(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// List the URI of all the blobs present in a container
        /// </summary>
        /// <param name="containerName">Blob container name</param>
        /// <returns>List of blobs URIs</returns>
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

        /// <summary>
        /// Download a file from blob storage
        /// </summary>
        /// <param name="containerName">Blob container name</param>
        /// <param name="blobName">Blob name</param>
        /// <param name="path">Path to download the file</param>
        /// <param name="fileMode">File mode</param>
        /// <returns></returns>
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

        /// <summary>
        /// Read blob as stream
        /// </summary>
        /// <param name="containerName">Blob container name</param>
        /// <param name="blobName">Blob name</param>
        /// <returns>Stream containing the blob content</returns>
        public async Task<Stream> ReadAsStreamAsync(string containerName, string blobName)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(containerName);
                var blockBlob = blobContainer.GetBlobReference(blobName);
                return await blockBlob.OpenReadAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete blob container
        /// </summary>
        /// <param name="containerName">Blob container name to delete</param>
        /// <returns>Task object</returns>
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
