using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Bootstrap
{
    /// <summary>
    /// Entry point of Prime.
    /// </summary>
    class Program
    {
        private static readonly List<PrimeBootstrapped> Entries = new List<PrimeBootstrapped>();

        static void Main(string[] args)
        {
            Console.WriteLine("Prime Bootstrapper: Entering..");

            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                CurrentDomain_ProcessExit();
            };

            Console.CancelKeyPress += Console_CancelKeyPress;

            var entry = new PrimeBootstrapped();
            Entries.Add(entry);
            var task = Task.Factory.StartNew(()=> entry.Enter(args));

            Task.WaitAll(task);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            CurrentDomain_ProcessExit();
        }

        static void CurrentDomain_ProcessExit()
        {
            if (Entries.All(x => x.IsShutdown))
                return;

            if (Entries.Any(x => !x.IsStopping))
                Console.WriteLine("Prime Bootstrapper: Stopping threads.");

            foreach (var e in Entries)
                e.IsStopping = true;
            
            do
            {
            } while (Entries.Any(x => !x.IsShutdown));

            try
            {
                Console.WriteLine("Prime Bootstrapper: Shutdown complete.");
            }
            catch { }
        }
    }
}
