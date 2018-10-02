using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Prime.Base.Messaging.Common;

namespace Prime.Core
{
    /// <summary>
    /// Entry point of Prime.
    /// </summary>
    public class BootstrapperEntry
    {
        internal static object BootListener = new object();

        public static void Enter(string[] args, string configPath)
        {
            var c = new PrimeContext(configPath) { L = new ConsoleLogger() };
            var prime = new PrimeInstance(c);

            prime.L.Log("Entered via Bootstrap with config: " + configPath);
            WriteEntranceHeader(prime);

            prime.Start();
            Final(prime, args, configPath);
        }

        public static void EnterSecondary(PrimeInstance prime, string[] args, string configPath)
        {
            Final(prime, args, configPath);
        }

        private static void Final(PrimeInstance prime, string[] args, string configPath)
        {
            CatchProcessExit(prime);

            prime.CommandArgs.Process(args);
            
            prime.Stop();

            prime.L.Log("Bootstrap exited.");
        }

        public static void WriteEntranceHeader(PrimeInstance prime)
        {
            var asm = Assembly.GetExecutingAssembly().GetName();
            prime.L.Log($"VERSION: {asm.Name} {Assembly.GetExecutingAssembly().GetName().Version}");
            prime.L.Log("");
        }

        private static void CatchProcessExit(PrimeInstance instance)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => CurrentDomain_ProcessExit(instance);
            Console.CancelKeyPress += (o, args) => CurrentDomain_ProcessExit(instance);
        }

        static void CurrentDomain_ProcessExit(PrimeInstance instance)
        {
            if (instance == null)
                return;

            if (!instance.IsStarted)
                return;

            instance.L.Log("Prime: Stop signal detected...");
            instance.Stop();
        }

        /*
        private static int Pub(PrimeBootOptions.Publish options)
        {
            Console.WriteLine("Prime Publisher starting.");

            options.PubConfigPath = options.PubConfigPath.ResolveSpecial();

            c.M.SendAndWait<PrimePublishRequest, PrimePublishResponse>(new PrimePublishRequest() { PublisherConfigPath = options.PubConfigPath});

            prime.Stop();
            return 0;
        }

        private static int Update(PrimeBootOptions.Update options)
        {
            Console.WriteLine("Prime Update starting.");

            c.M.SendAndWait<PrimeUpdateRequest, PrimeUpdateResponse>(new PrimeUpdateRequest() {  });

            prime.Stop();
            return 0;
        }

        private static int Packages(PrimeBootOptions.Packages options)
        {
            Console.WriteLine("Prime packages starting.");

            c.M.SendAndWait<PrimePackagesRequest, PrimePackagesResponse>(new PrimePackagesRequest(options) { });

            prime.Stop();
            return 0;
        }*/
    }
}
