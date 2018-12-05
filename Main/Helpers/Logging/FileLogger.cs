using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Main
{
    public class FileLogger : ILogger
    {
        public string FileName { get; }

        public string  Path { get; }

        private FileStream stream;

        public FileLogger(string fileName, string path = null)
        {
            if(string.IsNullOrEmpty(path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            path = $"{path}/{fileName}";
            FileName = fileName;
            Path = path;
            stream = File.Create(path);
        }

        public void Log(Exception exception)
        {
            var ex = JsonConvert.SerializeObject(exception);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"");
            builder.AppendLine($"{DateTime.Now}");
            builder.AppendLine("-- Exception Happened--");
            builder.AppendLine($"{ex}");
            builder.AppendLine($"");
            //File.WriteAllText(Path, builder.ToString());
            File.AppendAllText(Path, builder.ToString());
        }

        public void Log(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"");
            builder.AppendLine($"{DateTime.Now}");
            builder.AppendLine($"{message}");
            File.AppendAllText(Path, builder.ToString());
        }
    }
}
