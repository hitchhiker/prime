using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Prime.Base;
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

            WriteEntranceHeader(prime);
            prime.L.Log("Entered via Bootstrap with config: " + configPath);
            prime.L.Log("Args: " + string.Join(" ", args));

            prime.Start();
            Final(prime, args);
        }

        public static void EnterSecondary(PrimeInstance prime, string[] args)
        {
            Final(prime, args);
        }

        private static void Final(PrimeInstance prime, string[] args)
        {
            CatchProcessExit(prime);

            prime.CommandArgs.Process(args);
            
            prime.Stop();

            prime.L.Log("Bootstrap exited.");
        }

        public static void WriteEntranceHeader(PrimeInstance prime)
        {
            var asm = Assembly.GetExecutingAssembly().GetName();
            prime.L.Log($"{asm.Name} {asm.Version}".ToUpper());
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
    }
}
