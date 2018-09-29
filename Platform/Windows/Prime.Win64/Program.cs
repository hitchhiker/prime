using System;
using Prime.Bootstrap;

namespace Prime.Win64
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new[] { @"D:\hh\scratch\prime\prime-booted.config" };
            Bootstrapper.Boot(args);
        }
    }
}
