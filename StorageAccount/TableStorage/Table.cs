using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StorageAccount.TableStorage.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageAccount.TableStorage
{
    public class Table
    {
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;

        /// <summary>
        /// Table storage connection string
        /// </summary>
        public string ConnectionString { get; }

        public Table(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("No connection string found");
            }
            else if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                ConnectionString = connectionString;
                tableClient = storageAccount.CreateCloudTableClient();
            }
            else
            {
                throw new Exception("A connection string has not been defined in the system environment variables. Add a environment variable named 'storageconnectionstring' with your storage connection string as a value");
            }
        }

        /// <summary>
        /// Creates a table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>True if created successfully else false</returns>
        public async Task<bool> CreateTableAsync(string tableName)
        {
            try
            {
                var table = tableClient.GetTableReference(tableName);
                return await table.CreateIfNotExistsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts entity into table storage
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="entity">Entity to insert</param>
        /// <returns>True if created successfully else false</returns>
        public async Task<bool> InsertAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                var table = tableClient.GetTableReference(entity.TableName);
                var exists = await table.ExistsAsync();
                if (!exists)
                {
                    throw new Exception($"No table with {entity.TableName} exists");
                }
                return await InsertDataAsync(table, entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts entity into table storage
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="tableName">Table name</param>
        /// <param name="entity">Entity to insert</param>
        /// <returns>True if inserted successfully else false</returns>
        public async Task<bool> InsertAsync<T>(string tableName, T entity) where T : BaseEntity
        {
            try
            {
                var table = tableClient.GetTableReference(tableName);
                var exists = await table.ExistsAsync();
                if (!exists)
                {
                    throw new Exception($"No table with {entity.TableName} exists");
                }
                return await InsertDataAsync(table, entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts entities into table storage
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="entities">List of entities</param>
        /// <returns>True if inserted successfully else false</returns>
        public async Task<bool> InsertBulkAsync<T>(List<T> entities) where T : BaseEntity
        {
            try
            {
                var table = tableClient.GetTableReference(entities[0].TableName);
                var exists = await table.ExistsAsync();
                if (!exists)
                {
                    throw new Exception($"No table with {entities[0].TableName} exists");
                }
                TableBatchOperation batchOperations = new TableBatchOperation();
                foreach (var entity in entities)
                {
                    batchOperations.Insert(entity);
                }

                var status = await table.ExecuteBatchAsync(batchOperations);
                if(status.Count > 0)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete entity from table storage
        /// </summary>
        /// <typeparam name="T">Type of the table</typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <returns>True if deleted successfully else false</returns>
        public async Task<bool> DeleteEntityAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                var table = tableClient.GetTableReference(entity.TableName);
                var exists = await table.ExistsAsync();
                if (!exists)
                {
                    throw new Exception($"No table with {entity.TableName} exists");
                }
                var retriveOperation = TableOperation.Retrieve(entity.PartitionKey, entity.RowKey);
                var retriveResult = await table.ExecuteAsync(retriveOperation);

                var entityToDelete = (T)retriveResult.Result;
                if(entityToDelete != null)
                {
                    var deleteOperation = TableOperation.Delete(entityToDelete);
                    var status = await table.ExecuteAsync(deleteOperation);
                    if (status.HttpStatusCode == 200)
                    {
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        return await Task.FromResult(false);
                    }
                }
                else
                {
                    throw new Exception("Entity not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes table from storage
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>True if deleted successfully else false</returns>
        public async Task<bool> DeleteTableAsync(string tableName)
        {
            try
            {
                var table = tableClient.GetTableReference(tableName);
                return await table.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> GetDataAsync<T>(string tableName, string partitionKey, string rowKey, List<string> selectColumns = null) where T : BaseEntity
        {
            var table = tableClient.GetTableReference(tableName);
            var tableOperation = TableOperation.Retrieve<T>(partitionKey, rowKey, selectColumns);
            var status = await table.ExecuteAsync(tableOperation);
            if (status.HttpStatusCode == 200)
            {
                return (T) status.Result;
            }
            else
            {
                throw new TableException(status);
            }
        }

        public async Task<bool> InsertBulkElasticAsync<T>(string tableName, List<T> entities) where T : ElasticTableEntity
        {
            try
            {
                var table = tableClient.GetTableReference(tableName);
                var exists = await table.ExistsAsync();
                if (!exists)
                {
                    throw new Exception($"No table with {tableName} exists");
                }
                TableBatchOperation batchOperations = new TableBatchOperation();
                foreach (var entity in entities)
                {
                    batchOperations.Insert(entity);
                }

                var status = await table.ExecuteBatchAsync(batchOperations);
                if (status.Count > 0)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ElasticTableEntity>> GetDataElasticAsync(string tableName)
        {
            var table = tableClient.GetTableReference(tableName);
            var query = new TableQuery<ElasticTableEntity>();
            TableContinuationToken token = null;
            var items = new List<ElasticTableEntity>();
            do
            {
                var status = await table.ExecuteQuerySegmentedAsync(query, token);
                token = status.ContinuationToken;
                items.AddRange(status);
            }
            while (token != null);

            return items;
        }

        /// <summary>
        /// Inserts the entity into table storage
        /// </summary>
        /// <typeparam name="T">Type of the table</typeparam>
        /// <param name="table">Table reference</param>
        /// <param name="entity">Data to insert</param>
        /// <returns>True if inserted successfully else false</returns>
        private async Task<bool> InsertDataAsync<T>(CloudTable table, T entity) where T : BaseEntity
        {
            try
            {
                var tableOperation = TableOperation.Insert(entity);
                var status = await table.ExecuteAsync(tableOperation);
                if (status.HttpStatusCode == 200)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
