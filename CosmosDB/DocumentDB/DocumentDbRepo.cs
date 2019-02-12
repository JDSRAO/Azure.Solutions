using CosmosDB.DocumentDB.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.DocumentDB
{
    public class DocumentDbRepo<T> where T : Entity
    {
        protected internal DocumentClient Client;
        protected internal string PartitionKey;
        protected internal string CollectionId;
        protected internal string DatabaseId;

        public DocumentDbRepo(string endpoint, string authKey, string databaseId)
        {
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(authKey) || string.IsNullOrEmpty(databaseId))
            {
                throw new ArgumentNullException();
            }
            else
            {
                var serviceEndpoint = new Uri(endpoint);
                var DatabaseUri = UriFactory.CreateDatabaseUri(databaseId);
                Client = new DocumentClient(serviceEndpoint, authKey, new ConnectionPolicy { EnableEndpointDiscovery = false });
                CollectionId = Util.GetCollectionName<T>();
                PartitionKey = Util.GetPartitionKeyName<T>();
                DatabaseId = databaseId;

                Database db = Client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;
                var collection = new DocumentCollection { Id = CollectionId };
                collection.PartitionKey.Paths.Add("/" + PartitionKey);
                DocumentCollection coll = Client.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, collection).Result;
            }
        }

        public async Task DeleteCollectionAsync(string collectionId)
        {
            try
            {
                await Client.DeleteDocumentCollectionAsync(GetDocumentCollectionUri(collectionId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        #region Private Methods

        private Uri GetDocumentCollectionUri(string collectionid)
        {
            return UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionid);
        }

        private Uri GetDocumentUri(string documentid, string collectionId)
        {
            return UriFactory.CreateDocumentUri(DatabaseId, collectionId, documentid);
        }

        public async Task<T> CreateItemAsync(T item)
        {
            item.createdon = DateTime.UtcNow;
            item.modifiedon = DateTime.UtcNow;
            Document document = await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
            return (T)(dynamic)document;
        }

        public async Task<T> UpdateItemAsync(string id, T item)
        {
            item.modifiedon = DateTime.UtcNow;
            Document document = await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
            return (T)(dynamic)document;
        }

        public async Task<T> DeleteItemAsync(string id, T item)
        {
            Document document = await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
            return (T)(dynamic)document;
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                var documents = await GetItemsAsync(x => x.id.Equals(id));
                return documents.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query;
            if (predicate != null)
            {
                query = Client.CreateDocumentQuery<T>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                        new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                    .Where(predicate)
                    .AsDocumentQuery();
            }
            else
            {
                query = Client.CreateDocumentQuery<T>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                        new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })

                    .AsDocumentQuery();
            }

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }
        #endregion
    }


}
