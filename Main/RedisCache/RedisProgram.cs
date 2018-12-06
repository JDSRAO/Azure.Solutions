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
            Console.WriteLine("Cache program starting");
            cache.StringSet("key1", "jds-rao");
            var value = cache.StringGet("key1");
            Console.WriteLine($"{value}");
            Console.ReadKey();
            Console.WriteLine("Press any key to proceed");
        }
    }
}
