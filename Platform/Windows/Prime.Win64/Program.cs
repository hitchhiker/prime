using System;
using Prime.Bootstrap;

namespace Prime.Win64
{
    class Program
    {
        static int Main(string[] args)
        {
            return Bootstrapper.Boot(args, "Prime.exe");
        }
    }
}
