using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    public interface ILogger
    {
        void Log(Exception exception);
        void Log(string message);
    }
}
