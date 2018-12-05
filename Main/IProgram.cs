using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    interface IProgram
    {
        ILogger Logger { get; set; }
        void Run();
    }
}
