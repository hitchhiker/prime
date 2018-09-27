using System;
using System.Threading;
using Prime.Console.Frank;
using Prime.Console.Windows.Alyasko;
using Prime.Core;
using Prime.Core.Messaging;

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
                Frank.Go(server, client);
            }

            //Thread.Sleep(10000);
        }
    }
}
