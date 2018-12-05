using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    public static class  Logger
    {
        public static void Log(Exception exception)
        {
            string ex = JsonConvert.SerializeObject(exception);
            Console.WriteLine("-- Exception Happened--");
            Console.WriteLine($"{ex}");
            Console.WriteLine("");
        }

        public static void Log(string message)
        {
            Console.WriteLine("");
            Console.WriteLine($"{message}");
            Console.WriteLine("");
        }
    }
}
