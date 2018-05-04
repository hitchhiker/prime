using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Prime.Core;

namespace Prime.PackageManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            var workspaceDirectory = "./";
            var isPrime = false;

            if (args != null && args.Length > 0)
                workspaceDirectory = args[0];

            var logger = new ConsoleLogger() {IncludePreamble = false};

            if (Debugger.IsAttached)
            {
                workspaceDirectory = "V:\\prime\\src\\publish\\";
            }
            else
            {
                if (args == null || args.Length < 1)
                {
                    logger.Info("Usage: [workspacePath] [-m]");
                    return;
                }

                workspaceDirectory = args[0];

                isPrime = args.Length > 1 && args[1] == "-m";
            }

            workspaceDirectory = Path.GetFullPath(workspaceDirectory);

            logger.Info("Current: " + Path.GetFullPath("./"));

            var sourceDir = new DirectoryInfo(workspaceDirectory);
            if (!sourceDir.Exists)
            {
                logger.Info("No such directory: " + workspaceDirectory);
                return;
            }

            logger.Info("Workspace: " + workspaceDirectory);

            var pm = new PackageManager(new PackageManagerContext()
            {
                WorkspaceDirectory = new DirectoryInfo(workspaceDirectory),
                Logger = logger
            });

            pm.Process();

            logger.Info("");

            if (Debugger.IsAttached)
                Console.ReadLine();
        }
    }
}
