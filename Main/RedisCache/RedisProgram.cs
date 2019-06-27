using Redis.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main.RedisCache
{
    public class RedisProgram : IProgram
    {
        Cache cache;

        public RedisProgram()
        {
            cache = new Cache(AppSettings.CacheConnectionString);
        }

        public ILogger Logger { get; set; }

        public void Run()
        {
            var key = "key1";
            var value = "jds-rao";
            Console.WriteLine("Cache program starting");
            Console.WriteLine($"Setting key : {key}, value = {value} ");
            cache.StringSet(key, value);
            var valueFromCache = cache.StringGet("key1");
            Console.WriteLine($"Get value for key : {key}, value = {value}  from Redis as {valueFromCache}");
            Console.WriteLine("Press any key to proceed");
            Console.ReadKey();
        }
    }
}
