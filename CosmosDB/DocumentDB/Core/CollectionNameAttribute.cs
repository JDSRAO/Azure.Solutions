using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.DocumentDB.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionNameAttribute : Attribute
    {
        public virtual string Name { get; private set; }
        public virtual string PartitionKey { get; private set; }

        public CollectionNameAttribute(string value, string partitionKey)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            this.Name = value;
            this.PartitionKey = partitionKey;
        }
    }
}
