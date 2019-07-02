using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageAccount.TableStorage
{
    public abstract class BaseEntity : TableEntity
    {
        [IgnoreProperty]
        public string TableName { get; }
        //[IgnoreProperty]
        //public string PartitionKeyValue { get => PartitionKey; set => PartitionKey = value; }
        //[IgnoreProperty]
        //public string RowKeyValue { get => RowKey; set => RowKey = value; }

        public BaseEntity(string tableName)
        {
            TableName = tableName;
        }
    }
}
