using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Main
{
    public class FileLogger : ILogger
    {
        public string LogFilePath { get; }

        private FileStream stream;
        private StreamWriter streamWriter;

        public FileLogger(string fileName, string path = null)
        {
            if(string.IsNullOrEmpty(path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            path = Path.Combine(path, fileName);
            LogFilePath = path;
            try
            {
                //stream = File.Create(path);
                stream = File.OpenWrite(path);
                streamWriter = new StreamWriter(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }   
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
            File.AppendAllText(LogFilePath, builder.ToString());
            //stream.WriteByte(ToByte(builder.ToString()));
        }

        public void Log(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"");
            builder.AppendLine($"{DateTime.Now}");
            builder.AppendLine($"{message}");
        }

        private byte[] ToByte(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
