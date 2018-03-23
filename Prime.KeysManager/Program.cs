using System;
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
            Console.WriteLine($"Operating system: " + Environment.OSVersion.Platform);
            Console.WriteLine($"Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine($"UI app directory: " + ConfigManager.AppConfig.UiAppFolderPath);

            // Start server.
            var t = RunServer();

            // Start UI.
            RunUi();

            t.Wait();
        }

        private static Task RunServer()
        {
            var t = Task.Run(() =>
            {
                KeysManagerApp.Run();
            });
            Console.WriteLine("> Server started");

            return t;
        }

        private static void RunUi()
        {
            var uiProcess = new Process();
            
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                uiProcess.StartInfo.FileName = "/bin/bash";
                uiProcess.StartInfo.Arguments = "-c \"npm start\"";
                uiProcess.StartInfo.WorkingDirectory = "Electron";//;
                uiProcess.StartInfo.RedirectStandardOutput = true;
                //uiProcess.StartInfo.RedirectStandardError = true;
                uiProcess.StartInfo.UseShellExecute = false;
                //uiProcess.StartInfo.CreateNoWindow = false;
            }
            else
            {
                uiProcess.StartInfo.FileName = "cmd";
                uiProcess.StartInfo.Arguments = "/C npm start";
                uiProcess.StartInfo.WorkingDirectory = "Electron"; //ConfigManager.AppConfig.UiAppFolderPath;
            }
            
            //uiProcess.Start();
        }
    }
}