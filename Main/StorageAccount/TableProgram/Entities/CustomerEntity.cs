using Microsoft.WindowsAzure.Storage.Table;
using StorageAccount.TableStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Main.StorageAccount.TableProgram.Entities
{
    public class CustomerEntity : BaseEntity
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public CustomerEntity() : base("customerData")
        {

        }

        public CustomerEntity(string department) : base("customerData")
        {
            PartitionKey = department;
            RowKey = Guid.NewGuid().ToString();
        }

        public static explicit operator CustomerEntity(DynamicTableEntity dynamicTableEntity)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var convertedItem = new CustomerEntity();

            var dataProperties = convertedItem.GetType().GetProperties(flags);
            var dynamicTableEntityProperties = dynamicTableEntity.GetType().GetProperties();
            foreach (var property in dataProperties)
            {
                if (property.CanWrite)
                {
                    dynamic currentValue = currentValue = dynamicTableEntityProperties.Where(x => x.Name == property.Name).FirstOrDefault().GetValue(dynamicTableEntity);
                    //if (property.PropertyType == typeof(DateTime))
                    //{
                    //    currentValue = dynamicTableEntityProperties.Where(x => x.Name == property.Name).FirstOrDefault().GetValue(dynamicTableEntity);
                    //}
                    //else if (property.PropertyType == typeof(Nullable<DateTime>))
                    //{
                    //    currentValue = v.GetPropertyValue<Nullable<DateTime>>(property.Name);
                    //}
                    //else if (property.PropertyType == typeof(Nullable<TimeSpan>))
                    //{
                    //    currentValue = v.GetPropertyValue<Nullable<TimeSpan>>(property.Name);
                    //}
                    //else if (property.PropertyType == typeof(String))
                    //{
                    //    currentValue = v.GetPropertyValue<string>(property.Name);
                    //}
                    //else if (property.PropertyType == typeof(long))
                    //{
                    //    currentValue = v.GetPropertyValue<long>(property.Name);
                    //}
                    //else
                    //{
                    //    throw new InvalidOperationException($"No type convertor found for property {property.Name} and type {property.PropertyType}");
                    //}

                    property.SetValue(convertedItem, currentValue);
                }
            }

            return convertedItem;
        }
    }
}
