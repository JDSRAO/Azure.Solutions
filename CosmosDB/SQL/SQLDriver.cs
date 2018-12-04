using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace CosmosDB.SQL
{
    public class SQLDriver
    {
        public string DatabaseId { get; }
        public string CollectionId { get; set; }
        public Uri DatabaseUri { get;}

        private static DocumentClient client;
        

        public SQLDriver(string endpoint, string authKey, string databaseId)
        {
            
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(authKey))
            {
                throw new ArgumentNullException();
            }
            else
            {
                var serviceEndpoint = new Uri(endpoint);
                DatabaseId = databaseId;
                DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseId);
                client = new DocumentClient(serviceEndpoint, authKey);
            }
        }

        public SQLDriver(Uri endpoint, string authKey, string databaseId)
        {
            if (string.IsNullOrEmpty(endpoint.ToString()) || string.IsNullOrEmpty(authKey))
            {
                throw new ArgumentNullException();
            }
            else
            {
                DatabaseId = databaseId;
                DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseId);
                client = new DocumentClient(endpoint, authKey);
            }
        }

        public async Task CreateDatabaseAsync()
        {
            try
            {
                var database = new Database { Id = DatabaseId };
                var status = await client.CreateDatabaseAsync(database);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateDatabseIfNotExistsAsync()
        {
            try
            {
                await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task CreateCollectionAsync(string collectionId, int throughtPut = 400)
        {
            try
            {
                await client.CreateDocumentCollectionAsync(DatabaseUri, new DocumentCollection { Id = collectionId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateCollectionIfNotExistsAsync(string collectionId, int throughtPut = 400)
        {
            try
            {
                await client.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, new DocumentCollection { Id = collectionId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> GetAllCollections()
        {
            try
            {
                var dbs = await client.ReadDocumentCollectionFeedAsync(DatabaseUri);
                return dbs.Select(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteCollectionAsync(string collectionId)
        {
            try
            {
                await client.DeleteDocumentCollectionAsync(GetDocumentCollectionUri(collectionId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> GetDocumentAsync<T>(string id)
        {
            try
            {
                var documentURl = GetDocumentUri(id);
                var document = await client.ReadDocumentCollectionAsync(documentURl);
                return (T)(dynamic)document;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<T>> GetAllDocumentsAsync<T>(string collectionId = null)
        {
            dynamic documents = await client.ReadDocumentFeedAsync(GetDocumentCollectionUri(collectionId), new FeedOptions { MaxItemCount = 10 });
            var documents0 = client.CreateDocumentQuery<T>(GetDocumentCollectionUri(collectionId));
            foreach (var document in documents0)
            {
                Console.WriteLine($"{document}");
            }
            dynamic documents1 = client.CreateDocumentQuery<T>(GetDocumentCollectionUri(collectionId)).Select(x => JsonConvert.DeserializeObject<T>(x.ToString())).ToList();
            List<T> data = new List<T>();
            var test = (List<T>)documents;
            foreach (var document in documents)
            {
                data.Add((T)document);
            }
            return data;
        }

        public async Task InsertDocumentAsync<T>(T document, string collectionId = null)
        {
            try
            {
                await client.UpsertDocumentAsync(GetDocumentCollectionUri(collectionId), document);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateDocumentAsync<T>(string id, T document)
        {
            try
            {
                var documentUrl = GetDocumentUri(id);
                await client.ReplaceDocumentAsync(documentUrl, document);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task DeleteDocumentAsync(string id)
        {
            try
            {
                var documentUrl = GetDocumentUri(id);
                await client.DeleteDocumentAsync(documentUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private Methods

        private Uri GetDocumentCollectionUri(string collectionid = null)
        {
            if(string.IsNullOrEmpty(collectionid))
            {
                return UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            }
            else
            {
                return UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionid);
            }
            
        }

        private Uri GetDocumentUri(string documentid)
        {
            return UriFactory.CreateDocumentUri(DatabaseId, CollectionId, documentid);
        }

        private async Task<bool> CheckIfDatabaseExists()
        {
            try
            {
                var result = await client.ReadDatabaseAsync(DatabaseUri);
                if(result != null)
                {
                    return true;
                }
                return false;
            }
            catch(DocumentClientException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> CheckIfCollectionExists(string collectionid)
        {
            try
            {
                var result = await client.ReadDocumentCollectionAsync(GetDocumentCollectionUri(collectionid));
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
