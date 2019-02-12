using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.DocumentDB.Core
{
    public interface IEntity<TKey>
    {
        TKey id { get; set; }
    }
}
