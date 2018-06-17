using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Finance;
using Prime.KeysManager;
using Prime.Manager.Utils;
using Prime.MessagingServer;

namespace Prime.Manager.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger() { IncludePreamble = true };
            
            logger.Log("Operating system: " + Environment.OSVersion.Platform);
            logger.Log("Current directory: " + Environment.CurrentDirectory);
            
            var sCtx = new ServerContext()
            {
                L = logger
            };
            
            // Run Prime.
            
            var prime = new Prime.Core.Prime(sCtx);
            prime.Extensions.Loader.LoadAllBinDirectoryAssemblies();
            prime.Extensions.LoadInstalled();

            sCtx.Assemblies.Refresh();
            sCtx.Types.Refresh();
            
            // Create MessagingServer and run ManagerExtension.
            
            var server = new MessagingServer.Server(sCtx);
            
            var managerExt = new ManagerServiceExtension();
            managerExt.Main(sCtx);

            server.Start();

            logger.Log("Server started");
            
            // Start UI.
            
            logger.Log("UI started");
            logger.Log("Waiting for all UI processes exit...");

            //RunUiAsync(logger).Wait();
            
            Console.ReadLine();
        }

        private static Task RunUiAsync(ILogger logger)
        {
            return Task.Run(() =>
            {
                var process = RunUi();

                logger.Log($": Electron UI started ({process.Id}).");
                process.WaitForExit();
            });
        }

        private static Process RunUi()
        {
            var uiProcess = new Process();

            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                uiProcess.StartInfo.FileName = "/bin/bash";
                uiProcess.StartInfo.Arguments = "-c \"npm run electron\"";
                uiProcess.StartInfo.WorkingDirectory = ElectronUtils.FindElectronUiDirectory("Electron", Environment.CurrentDirectory);
                uiProcess.StartInfo.RedirectStandardOutput = true;
                uiProcess.StartInfo.UseShellExecute = false;
            }
            else
            {
                uiProcess.StartInfo.FileName = "cmd";
                uiProcess.StartInfo.Arguments = "/C npm run electron";
                uiProcess.StartInfo.RedirectStandardOutput = true;
                uiProcess.StartInfo.WorkingDirectory = ElectronUtils.FindElectronUiDirectory("Electron", Environment.CurrentDirectory);
            }

            uiProcess.Start();
            return uiProcess;
        }
    }
}
