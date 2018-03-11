using System;
using System.Diagnostics;
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
        static void Main(string[] args)
        {
            Console.WriteLine($"Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine($"UI app directory: " + ConfigManager.AppConfig.UiAppFolderPath);

            // Start server.
            var t = Task.Run(() =>
            {
                var app = new KeysManagerApp(new TcpServer(), new PrimeService());
                app.Run();
            });
            Console.WriteLine("> Server started");

            // Start UI.
            var uiProcess = new Process();
            uiProcess.StartInfo.FileName = "cmd";
            uiProcess.StartInfo.Arguments = "/C npm start";
            uiProcess.StartInfo.WorkingDirectory = ConfigManager.AppConfig.UiAppFolderPath;
            uiProcess.Start();


            t.Wait();
        }
    }
}