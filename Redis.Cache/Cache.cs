using StackExchange.Redis;
using System;

namespace Redis.Cache
{
    public class Cache
    {
        /// <summary>
        /// Redis Cache connection string
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Lazy connection multiplexer
        /// </summary>
        private Lazy<ConnectionMultiplexer> lazyConnection;

        /// <summary>
        /// Connection object
        /// </summary>
        public ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public Cache(string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }
            ConnectionString = connectionString;
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = connectionString;
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        /// <summary>
        /// Set value in cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value to set</param>
        /// /// <param name="expiry">Time to live</param>
        public void StringSet(string key, string value, TimeSpan? expiry = null)
        {
            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }
            Connection.GetDatabase().StringSet(key, value, expiry);
        }

        /// <summary>
        /// Get string value from cache
        /// </summary>
        /// <param name="key">Key of the item to get value</param>
        /// <returns>String value from cache</returns>
        public string StringGet(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }
            return Connection.GetDatabase().StringGet(key).ToString();
        }
    }
}
