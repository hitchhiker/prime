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
            var workspacePath = "./";
            var isPrime = false;

            if (args != null && args.Length > 0)
                sourcePath = args[0];

            var logger = new ConsoleLogger() {IncludePreamble = false};

            if (Debugger.IsAttached)
            {
                sourcePath = "V:\\prime\\src\\Ext\\Prime.IPFS\\Prime.IPFS.Win64\\bin\\Debug";
                workspacePath = "V:\\prime\\src\\publish\\";
            }
            else
            {
                if (args == null || args.Length < 2)
                {
                    logger.Info("Usage: [sourcePath] [workspacePath] [-m]");
                    return;
                }

                sourcePath = args[0];
                workspacePath = args[1];

                isPrime = args.Length > 3 && args[3] == "-m";
            }

            sourcePath = Path.GetFullPath(sourcePath);
            workspacePath = Path.GetFullPath(workspacePath);

            logger.Info("Current: " + Path.GetFullPath("./"));

            var sourceDir = new DirectoryInfo(sourcePath);
            if (!sourceDir.Exists)
            {
                logger.Info("No such directory: " + sourcePath);
                return;
            }

            logger.Info("Source: " + sourcePath);
            logger.Info("Workspace: " + workspacePath);

            var ctx = new ProgramContext()
            {
                SourceDirectory = sourceDir,
                WorkspaceDirectory = new DirectoryInfo(workspacePath),
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
