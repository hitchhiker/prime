using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.ExtensionPackager
{
    public class Program
    {
        static void Main(string[] args)
        {
            ExtensionInfo.DummyRef();
            
            var extPath = "./";
            var configPath = "./";

            var isPrime = false;

            if (args != null && args.Length > 0)
                extPath = args[0];

            var logger = new ConsoleLogger() {IncludePreamble = false};

            if (Debugger.IsAttached)
            {
                configPath = "..\\..\\..\\..\\..\\..\\instance\\prime.config";
                extPath = "..\\..\\..\\..\\..\\..\\Ext\\Prime.IPFS\\Prime.IPFS.Win64\\bin\\Debug";
            }
            else
            {
                if (args == null || args.Length < 2)
                {
                    logger.Info("Usage: [configPath] [extPath] [-core]");
                    return;
                }

                configPath = args[0];
                extPath = args[1];

                isPrime = args.Length > 2 && args[3] == "-core";
            }

            configPath = Path.GetFullPath(configPath);
            extPath = Path.GetFullPath(extPath);

            var extDir = new DirectoryInfo(extPath);
            if (!extDir.Exists)
            {
                logger.Info("No such directory: " + extPath);
                return;
            }

            var pc = new ServerContext(configPath);

            logger.Info("config: " + configPath);
            logger.Info("extension: " + extPath);

            var ctx = new ProgramContext(pc)
            {
                SourceDirectory = extDir,
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
