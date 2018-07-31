using Prime.Core;
using System;
using System.Threading.Tasks;
using NetCrossRun.Core;
using Prime.Core.Testing;
using Prime.KeysManager;
using Prime.Settings;

namespace Prime
{
    /// <summary>
    /// Prime main console application.
    /// Runs TCP socket, WebSocket services.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Prime Core ---");
            var logger = new ConsoleLogger() {IncludePreamble = true};

            // Initialization.
            var serverContext = new ServerContext()
            {
                L = logger
            };

            serverContext.Assemblies.Refresh();
            serverContext.Types.Refresh();

            var messagingServer = new MessagingServer.Server(serverContext);
            
            // Run Prime Web.
            var primeWeb = new PrimeWeb()
            {
                L = logger
            };
            //primeWeb.Run();
            
            // Run TCP server for interproc communication with extensions.
            var primeTcp = new PrimeSocketServer(serverContext, messagingServer)
            {
                L = logger
            };
            primeTcp.Start();

            // Run WebSocket server.
            var primeWs = new PrimeWsServer(serverContext, messagingServer)
            {
                L = logger
            };
            primeWs.Start();
            
            // Demo messages handling.
            messagingServer.M.Register<UpdateTimeKindRequestMessage>(logger, (r) =>
            {
                messagingServer.M.Send(new UpdateTimeKindInternalRequestMessage()
                {
                    IsUtc = r.IsUtc
                });
            });
            
            messagingServer.M.Register<TimeKindUpdatedRequestMessage>(logger, (r) =>
            {
                messagingServer.M.SendAsync(new UpdateWebUiTimeKindRequestMessage()
                {
                    IsUtc = r.IsUtcTime
                });
            });
            //

            messagingServer.Start();

            // Wait for input.
            Console.ReadKey();
        }
        
        [Obsolete("Not used for the moment. Will be considered later.")]
        private void Run()
        {
            var logger = new ConsoleLogger() {IncludePreamble = true};

            logger.Log("OS: " + Environment.OSVersion.Platform);
            logger.Log("Current directory: " + Environment.CurrentDirectory);

            var sCtx = new ServerContext()
            {
                L = logger
            };

            sCtx.Assemblies.Refresh();
            sCtx.Types.Refresh();

            // Run Prime.
            var prime = new PrimeInstance(sCtx);
            prime.ExtensionManager.LoadInstallConfig();
            prime.Start();

            // Run Messaging Server.
            var server = new MessagingServer.Server(sCtx);

            server.M.Register<HelloRequest>(this, (request) => { logger.Log($"{request.GetType()} received."); });

            logger.Log("Prime started");

            server.Start();

            // HACK. Run WS server. Unable to load it using extension manager.
            RunWs(server, sCtx);
            // END HACK.

            // Run command processor.
            var commandProcessor = new CommandProcessor();
            commandProcessor.Bind("exit", (cmd) => { Environment.Exit(0); });


            Console.ReadKey();
            //commandProcessor.Start();
        }

        [Obsolete("Remove this test code!!!")]
        private void RunWs(MessagingServer.Server server, ServerContext sctx)
        {
            var manager = new ManagerServiceExtension();
            manager.Main(sctx);

            var wsServerExt = new Prime.WebSocketServer.ServerExtension();
            server.Inject(wsServerExt);
            wsServerExt.Start(server);
        }
    }
}