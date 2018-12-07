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

        public FileLogger(string fileName, string path = null)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                path = Path.Combine(path, fileName);
                LogFilePath = path;
                if(File.Exists(path))
                {
                    stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                }
                else
                {
                    stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                }
                
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
            builder.AppendLine($"-- {DateTime.Now}");
            builder.AppendLine($"{ex}");
            builder.AppendLine($"");
            stream.Write(ToByte(builder.ToString()));
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
