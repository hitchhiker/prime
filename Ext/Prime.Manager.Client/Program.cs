using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prime.Core;
using Prime.KeysManager;
using Prime.Manager.Utils;
using Prime.MessagingServer.Types;

namespace Prime.Manager.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger() { IncludePreamble = true };

            logger.Log("Operating system: " + Environment.OSVersion.Platform);
            logger.Log("Current directory: " + Environment.CurrentDirectory);

            logger.Log("Prime.Manager started");

            var serverContext = new ServerContext()
            {
                L = logger
            };

            var server = new MessagingServer.Server(serverContext);
            server.Start();

            var wsServerExtension = new WebSocketServer.ServerExtension();
            var managerExtension = new ManagerServiceExtension();
            managerExtension.Main(serverContext);

            server.Inject(wsServerExtension);

            wsServerExtension.Start(server);

            logger.Log("Ws server started");

            // Starting TCP client.
            logger.Log("Starting TCP client...");

            var client = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            logger.Log("Establishing connection to local socket server.");
            client.Connect("127.0.0.1", 9991);

            Console.ReadLine();

            //////////////// OLD /////////////

            //var logger = new ConsoleLogger() { IncludePreamble = true };

            //logger.Log("Operating system: " + Environment.OSVersion.Platform);
            //logger.Log("Current directory: " + Environment.CurrentDirectory);

            //logger.Log("Prime.Manager started");

            //var client = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //logger.Log("Establishing connection to local socket server.");
            //client.Connect("127.0.0.1", 9991);

            //// TODO: Alyasko: implement TCP client here.

            ////var server = new MessagingServer.Server();

            //var settings = new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.Objects,
            //    //SerializationBinder = server.TypeBinder
            //};

            //var dataString = "";//JsonConvert.SerializeObject(msg, settings);

            //logger.Log("Connection established, sending message: " + dataString);

            //var dataBytes = dataString.GetBytes();

            //client.Send(dataBytes);

            //Task.Run(() =>
            //{
            //    var helper = new MessageTypedSender(null); // TODO: Alyasko: fix this.

            //    do
            //    {
            //        var buffer = new byte[1024];
            //        var iRx = client.Receive(buffer);
            //        var recv = buffer.GetString().Substring(0, iRx);

            //        if (string.IsNullOrWhiteSpace(recv))
            //            continue;

            //        if (JsonConvert.DeserializeObject(recv, settings) is BaseTransportMessage m)
            //            helper.UnPackSendReceivedMessage(new ExternalMessage(m.SessionId, m));

            //    } while (client.Connected);
            //});

            //Console.ReadLine();
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
