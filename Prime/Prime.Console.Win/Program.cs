using System;
using System.Threading;
using Prime.Scratch.Alyasko;
using Prime.Core;
using Prime.Core.Messaging;
using Prime.Scratch;

namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serverCtx = PrimeContext.ForDevelopmentServer();
            var clientCtx = PrimeContext.ForDevelopmentClient();

            var server = new PrimeInstance(serverCtx);
            var client = new PrimeInstance(clientCtx);

            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                server.Stop();
                client.Stop();
            };

            Console.CancelKeyPress += (o, eventArgs) =>
            {
                server.Stop();
                client.Stop();
            };

            // if this is removed DEBUG wont work across projects!??
            var i = ClassTestBase.Test();
            i = i + ClassTestCore.Test();
            i = i + ClassTestPm.Test();
            // end HACK

            //serverCtx.Assemblies.Refresh();
            //serverCtx.Types.Refresh();

            var userName = Environment.UserName.ToLower();

            if (userName.Equals("yasko") || userName.Equals("alexander"))
            {
                AlyaskoTest.Go(serverCtx, clientCtx);
            }
            else if (userName.Equals("hitchhiker"))
            {
                FrankTest.Go(server, client);
            }

            //Thread.Sleep(10000);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
