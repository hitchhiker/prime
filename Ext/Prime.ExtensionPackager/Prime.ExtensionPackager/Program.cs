using System;
using System.Diagnostics;
using System.IO;

namespace Prime.ExtensionPackager
{
    public class Program
    {
        static void Main(string[] args)
        {
            var sourcePath = "./";
            var stagingDirectory = "./";
            var distDirectory = "./";
            var isPrime = false;

            if (args != null && args.Length > 0)
                sourcePath = args[0];

            if (Debugger.IsAttached)
            {
                sourcePath = "C:\\tmp\\packer\\src";
                stagingDirectory = "C:\\tmp\\packer\\stage";
                distDirectory = "C:\\tmp\\packer\\dist";
            }
            else
            {
                if (args == null || args.Length < 3)
                {
                    Console.WriteLine("Usage: [sourcePath] [stagePath] [distPath] [-m]");
                    return;
                }

                sourcePath = Path.GetFullPath(args[0]);
                stagingDirectory = Path.GetFullPath(args[1]);
                distDirectory = Path.GetFullPath(args[2]);
                isPrime = args.Length > 3 && args[3] == "-m";
            }

            Console.WriteLine("Current: " + Path.GetFullPath("./"));

            var sourceDir = new DirectoryInfo(sourcePath);
            if (!sourceDir.Exists)
            {
                Console.WriteLine("No such directory: " + sourcePath);
                return;
            }

            Console.WriteLine("Source: " + sourcePath);
            Console.WriteLine("Staging: " + stagingDirectory);
            Console.WriteLine("Distrubtion: " + distDirectory);

            var ctx = new ProgramContext()
            {
                SourceDirectory = sourceDir,
                StagingDirectory = new DirectoryInfo(stagingDirectory),
                DistributionDirectory = new DirectoryInfo(distDirectory),
                IsPrime = isPrime
            };

            Process.Go(ctx);

            Console.WriteLine("");
        }
    }
}
