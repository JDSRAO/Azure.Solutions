using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.DocumentDB.Core
{
    public static class Util
    {
        internal static string GetCollectionName<T>() where T : Entity
        {
            string collectionName;
            if (typeof(T).BaseType.Equals(typeof(object)))
            {
                collectionName = GetCollectionNameFromInterface<T>();
            }
            else
            {
                collectionName = GetCollectionNameFromType(typeof(T));
            }

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentException("Collection name cannot be empty for this entity");
            }
            return collectionName;
        }

        private static string GetCollectionNameFromInterface<T>()
        {
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameAttribute));
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionNameAttribute)att).Name;
            }
            else
            {
                collectionname = typeof(T).Name;
            }

            return collectionname;
        }

        public static string GetPartitionKeyName<T>()
        {
            string PartitionKeyName = "";

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameAttribute));
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                PartitionKeyName = ((CollectionNameAttribute)att).PartitionKey;
            }

            return PartitionKeyName;
        }

        private static string GetCollectionNameFromType(Type entitytype)
        {
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(entitytype, typeof(CollectionNameAttribute));
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionNameAttribute)att).Name;
            }
            else
            {
                if (typeof(Entity).IsAssignableFrom(entitytype))
                {
                    // No attribute found, get the basetype
                    while (entitytype?.BaseType != typeof(Entity))
                    {
                        entitytype = entitytype?.BaseType;
                    }
                }
                collectionname = entitytype.Name;
            }

            return collectionname;
        }
    }
}
