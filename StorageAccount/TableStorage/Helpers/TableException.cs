using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageAccount.TableStorage.Helpers
{
    public class TableException : Exception
    {
        private string CustomMessage { get; }

        public TableException()
        {

        }

        public TableException(string message) : base (message)
        {
            
        }

        public TableException(TableResult result) : base(ResolveTableStatus(result))
        {
            
        }

        private static string ResolveTableStatus(TableResult result)
        {
            return $"Error occurred while fetching results : statuc code {result.HttpStatusCode} \n Result : {result.Result}";
        }
    }
}
