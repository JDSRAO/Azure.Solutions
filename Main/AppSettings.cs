using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    public class AppSettings
    {
        #region Service Bus Settings

        public const string ServiceBusConnectionString = "Endpoint={namespace};SharedAccessKeyName=SWSharedAccessKey;SharedAccessKey=6gY8lEiwluap62tX2hF22CqT6ceC8U24P43pMOoMJm8=";
        public const string ServiceBusQueueName = "srinivas-playground";
        public const string ServiceBusTopicName = "srinivas-topic-playground";
        public const string ServiceBusTopicSubscriptionName = "srinivas-topic-subscription-playground";

        #endregion

        #region Redis Cache Settings

        public const string CacheConnectionString = "127.0.0.1:6379";

        #endregion

        #region Storage Account Settings

        public const string StorageAccountConnectionString = "UseDevelopmentStorage=true";
        public const string StorageAccountTableName = "customerData";
        public const string StorageAccountQueueame = "local-dev-playground";
        public const string StorageAccountBlobContainerName = "local-dev-playground";
        public const string StorageAccountBlobName = "quickStartBlob";

        #endregion

        #region Cosmos DB Settings

        public const string CosmosDB_MongoDBConnectionString = "mongodb://127.0.0.1:27017";
        public const string CosmosDB_MongoDBDatabase = "test";
        public const string CosmosDB_MongoDBCollection = "testcol";
        public const string CosmosDB_SQLConnectionString = "https://localhost:8081";
        public const string ComsomDB_SQLKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public const string CosmosDB_SQLDatabaseId = "SampleDatabase";

        #endregion
    }
}
