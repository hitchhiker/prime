using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using Prime.KeysManager.Core;
using Prime.KeysManager.Utils;
using Prime.MessagingServer;

namespace Prime.KeysManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var sCtx = new ServerContext("..//..//..//..//..//instance//prime-server.config");

            var logger = new ConsoleLogger() { IncludePreamble = true };

            sCtx.L = logger;

            logger.Log(": Operating system: " + Environment.OSVersion.Platform);
            logger.Log(": Current directory: " + Environment.CurrentDirectory);

            var server = new Server(sCtx);
            server.Start();

            foreach (var i in server.TypeBinder.TypeCatalogue)
                logger.Log(server.TypeBinder.TypeCatalogue.Get(i));

            // Start message listener.
            var keysManager = new KeyManagerServer(sCtx);

            Task.Run(() => { keysManager.Run(); });
            logger.Log(": Server started");

            // Start UI.
            var uiTask = Task.Run(() =>
            {
                var process = RunUi();

                logger.Log($": Electron UI started ({process.Id}).");
                process.WaitForExit();
            });

            logger.Log(": UI started");
            logger.Log(": Waiting for all UI processes exit...");

            uiTask.Wait();
        }

        private static Process RunUi()
        {
            var uiProcess = new Process();

            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                uiProcess.StartInfo.FileName = "/bin/bash";
                uiProcess.StartInfo.Arguments = "-c \"npm start\"";
                uiProcess.StartInfo.WorkingDirectory = ElectronUtils.FindElectronUiDirectory(ConfigManager.AppConfig.ElectronFolderName, Environment.CurrentDirectory);
                uiProcess.StartInfo.RedirectStandardOutput = true;
                uiProcess.StartInfo.UseShellExecute = false;
            }
            else
            {
                uiProcess.StartInfo.FileName = "cmd";
                uiProcess.StartInfo.Arguments = "/C npm start";
                uiProcess.StartInfo.RedirectStandardOutput = true;
                uiProcess.StartInfo.WorkingDirectory = ElectronUtils.FindElectronUiDirectory(ConfigManager.AppConfig.ElectronFolderName, Environment.CurrentDirectory);
            }
            
            uiProcess.Start();
            return uiProcess;
        }
    }
}