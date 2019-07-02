using System;
using System.IO;

namespace Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert string to stream
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Stream ToStream(this string str)
        {
            var result = new MemoryStream();

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                    stream.Position = 0;
                    stream.CopyTo(result);
                    result.Position = 0;
                }
            }

            return result;
        }
    }
}
