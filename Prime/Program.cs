using Prime.Core;
using System;
using Prime.Core.Testing;

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
            new Program().Run();
        }

        private void Run()
        {
            var logger = new ConsoleLogger() { IncludePreamble = true };

            logger.Log("OS: " + Environment.OSVersion.Platform);
            logger.Log("Current directory: " + Environment.CurrentDirectory);

            var sCtx = new ServerContext()
            {
                L = logger
            };

            // Run Prime.
            var prime = new PrimeInstance(sCtx);
            //prime.Extensions.Loader.LoadAllBinDirectoryAssemblies();
            //prime.Extensions.LoadInstalled();

            sCtx.Assemblies.Refresh();
            sCtx.Types.Refresh();

            // Run Messaging Server.
            var server = new MessagingServer.Server(sCtx);
            server.Start();

            server.M.Register<HelloRequest>(this, (request) =>
            {
                logger.Log($"{request.GetType()} received.");
            });

            logger.Log("Prime started");

            // Run command processor.
            var commandProcessor = new CommandProcessor();
            commandProcessor.Bind("exit", (cmd) =>
            {
                Environment.Exit(0);
            });

            commandProcessor.Start();
        }
    }
}
