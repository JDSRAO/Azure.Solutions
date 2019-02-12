using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.DocumentDB.Core
{
    public abstract class Entity : IEntity<string>
    {
        public virtual string id { get; set; }
        public DateTime createdon { get; set; }
        public string createdby { get; set; }
        public DateTime modifiedon { get; set; }
        public string modifiedby { get; set; }
        [JsonIgnore]
        public abstract string partitionkeyvalue { get; }
    }
}
