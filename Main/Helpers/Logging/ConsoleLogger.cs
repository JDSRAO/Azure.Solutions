using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
        }

        public void Log(Exception exception)
        {
            string ex = JsonConvert.SerializeObject(exception);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"");
            builder.AppendLine($"{DateTime.Now}");
            builder.AppendLine("-- Exception Happened--");
            builder.AppendLine($"{ex}");
            builder.AppendLine($"");
            Console.WriteLine(builder.ToString());
        }

        public void Log(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"");
            builder.AppendLine($"{DateTime.Now}");
            builder.AppendLine($"{message}");
            Console.WriteLine(builder.ToString());
        }
    }
}
