using System;
using System.Diagnostics;
using System.IO;
using Prime.Core;

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

            var logger = new ConsoleLogger() {IncludePreamble = false};

            if (Debugger.IsAttached)
            {
                sourcePath = "V:\\prime\\src\\Ext\\Prime.IPFS\\Prime.IPFS.Win64\\bin\\Debug";
                stagingDirectory = "V:\\prime\\src\\publish\\stage";
                distDirectory = "V:\\prime\\src\\publish\\dist";
            }
            else
            {
                if (args == null || args.Length < 3)
                {
                    logger.Info("Usage: [sourcePath] [stagePath] [distPath] [-m]");
                    return;
                }

                sourcePath = args[0];
                stagingDirectory = args[1];
                distDirectory = args[2];

                isPrime = args.Length > 3 && args[3] == "-m";
            }

            sourcePath = Path.GetFullPath(sourcePath);
            stagingDirectory = Path.GetFullPath(stagingDirectory);
            distDirectory = Path.GetFullPath(distDirectory);

            logger.Info("Current: " + Path.GetFullPath("./"));

            var sourceDir = new DirectoryInfo(sourcePath);
            if (!sourceDir.Exists)
            {
                logger.Info("No such directory: " + sourcePath);
                return;
            }

            logger.Info("Source: " + sourcePath);
            logger.Info("Staging: " + stagingDirectory);
            logger.Info("Distrubtion: " + distDirectory);

            var ctx = new ProgramContext()
            {
                SourceDirectory = sourceDir,
                StagingDirectory = new DirectoryInfo(stagingDirectory),
                DistributionDirectory = new DirectoryInfo(distDirectory),
                IsPrime = isPrime,
                Logger = logger
            };

            Process.Go(ctx);

            logger.Info("");

            if (Debugger.IsAttached)
                Console.ReadLine();
        }
    }
}
