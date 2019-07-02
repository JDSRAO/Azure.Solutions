using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.MongoDB.Driver
{
    public class MongoDriverSingleInstance
    {
        /// <summary>
        /// MongoDB connection string
        /// </summary>
        public string ConnectionString { get; }

        private MongoClient client;
        private IMongoDatabase database { get; }
        private string databaseName { get; }

        public MongoDriverSingleInstance(string connectionString, string database)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }
            ConnectionString = connectionString;
            client = new MongoClient(connectionString);
            databaseName = database;
            this.database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Gets all the databases in the server
        /// </summary>
        /// <returns>List of database names in the server</returns>
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

        /// <summary>
        /// Drops a database
        /// </summary>
        /// <returns>Task object</returns>
        public async Task DropDatabaseAsync()
        {
            try
            {
                await client.DropDatabaseAsync(databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates a collection 
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <returns></returns>
        public async Task CreateCollectionAsync(string collection)
        {
            try
            {
                await database.CreateCollectionAsync(collection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts data into collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection"></param>
        /// <param name="document">Document to insert</param>
        /// <returns></returns>
        public async Task InsertData<T>(string collection, T document)
        {
            try
            {
                IMongoCollection<T> dbCollection = GetCollection<T>(collection);
                await dbCollection.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all the documents from the collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<T>> FindAllDocuments<T>(string collection, Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                IMongoCollection<T> dbCollection = GetCollection<T>(collection);
                FilterDefinition<T> filter;
                if (predicate == null)
                {
                    filter = FilterDefinition<T>.Empty;
                }
                else
                {
                    filter = predicate;
                }
                var documents = await dbCollection.FindAsync<T>(filter);
                return documents.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all the collections from a database
        /// </summary>
        /// <returns>Collection names</returns>
        public async Task<List<string>> GetAllCollectionsAsync()
        {
            try
            {
                var collections = await database.ListCollectionNamesAsync();
                return collections.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Drops a collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task DropCollectionAsync(string collection)
        {
            try
            {
                await database.DropCollectionAsync(collection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private Methods

        private IMongoCollection<T> GetCollection<T>(string collection)
        {
            return database.GetCollection<T>(collection);
        }

        #endregion
    }
}
