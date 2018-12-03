using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.MongoDB.Driver
{
    public class MongoDriver
    {
        public string ConnectionString { get; }

        private MongoClient client;

        public MongoDriver(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }
            ConnectionString = connectionString;
            client = new MongoClient(connectionString);
        }

        public async Task<List<string>> GetAllDatabasesAsync()
        {
            try
            {
                var names = await client.ListDatabaseNamesAsync();
                return names.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DropDatabaseAsync(string database)
        {
            try
            {
                await client.DropDatabaseAsync(database);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateCollectionAsync(string database, string collection)
        {
            try
            {
                await GetDatabase(database).CreateCollectionAsync(collection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task InsertData<T>(string database, string collection, T document)
        {
            try
            {
                IMongoCollection<T> dbCollection = GetCollection<T>(database, collection);
                await dbCollection.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<T>> FindAllDocuments<T>(string database, string collection)
        {
            try
            {
                IMongoCollection<T> dbCollection = GetCollection<T>(database, collection);
                var filter = FilterDefinition<T>.Empty;
                var documents = await dbCollection.FindAsync<T>(filter);
                return documents.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> GetAllCollectionsAsync(string database)
        {
            try
            {
                var collections = await GetDatabase(database).ListCollectionNamesAsync();
                return collections.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DropCollectionAsync(string database, string collection)
        {
            try
            {
                await GetDatabase(database).DropCollectionAsync(collection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private Methods

        private IMongoDatabase GetDatabase(string database)
        {
            return client.GetDatabase(database);
        }

        private IMongoCollection<T> GetCollection<T>(string database, string collection)
        {
            return GetDatabase(database).GetCollection<T>(collection);
        }

        #endregion
    }
}
