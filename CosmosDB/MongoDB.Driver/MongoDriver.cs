using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.MongoDB.Driver
{
    public class MongoDriver
    {
        /// <summary>
        /// MongoDB connection string
        /// </summary>
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
        /// <param name="database">Database to drop</param>
        /// <returns>Task object</returns>
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

        /// <summary>
        /// Creates a collection 
        /// </summary>
        /// <param name="database">Database name</param>
        /// <param name="collection">Collection name</param>
        /// <returns></returns>
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

        /// <summary>
        /// Inserts data into collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <param name="document">Document to insert</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all the documents from the collection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<T>> FindAllDocuments<T>(string database, string collection, Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                IMongoCollection<T> dbCollection = GetCollection<T>(database, collection);
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
        /// <param name="database"></param>
        /// <returns>Collection names</returns>
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

        /// <summary>
        /// Drops a collection
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
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
