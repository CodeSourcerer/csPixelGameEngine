using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using csPixelGameEngine;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            GLWindow window = new GLWindow(500, 500, "Test Application");
            window.Run();
        }
    }
}
