using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Prime.KeysManager.Core;
using Prime.KeysManager.Transport;
using Prime.KeysManager.Utils;

namespace Prime.KeysManager
{
    class Program
    {
        private static readonly KeysManagerApp KeysManagerApp = new KeysManagerApp(new TcpServer(), new PrimeService());
        
        static void Main(string[] args)
        {
            Console.WriteLine($"> Operating system: " + Environment.OSVersion.Platform);
            Console.WriteLine($"> Current directory: " + Environment.CurrentDirectory);

            // Start server.
            RunServer();
            Console.WriteLine("> Server started");

            // Start UI.
            var command = "";
            var uiTasks = new List<Task>();

            if (false)
            {
                while (!command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("> Enter command: ");
                    command = Console.ReadLine();

                    if (command.Equals("ui", StringComparison.OrdinalIgnoreCase))
                    {
                        uiTasks.Add(RunUiAsync());
                    }
                }
            }

            uiTasks.Add(RunUiAsync());

            Console.WriteLine("> Waiting for all UI processes exit...");
            Console.ReadLine();
            Task.WaitAll(uiTasks.ToArray());
        }

        private static Task RunServer()
        {
            return Task.Run(() =>
            {
                KeysManagerApp.Run();
            });
        }

        private static Task RunUiAsync()
        {
            return Task.Run(() =>
            {
                var process = RunUi();

                Console.WriteLine($"> Electron UI started ({process.Id}).");
                process.WaitForExit();
            });
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