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
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        /// <summary>
        /// Connection object
        /// </summary>
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public Cache(string connectionString)
        {
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
            Connection.GetDatabase().StringSet(key, value, expiry);
        }

        /// <summary>
        /// Get string value from cache
        /// </summary>
        /// <param name="key">Key of the item to get value</param>
        /// <returns>String value from cache</returns>
        public string StringGet(string key)
        {
            return Connection.GetDatabase().StringGet(key).ToString();
        }
    }
}
