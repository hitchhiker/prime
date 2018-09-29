using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace Prime.Core
{
    /// <summary>
    /// Entry point of Prime.
    /// </summary>
    public class BootstrapperEntry
    {
        private static readonly List<PrimeEntryMarshal> Entries = new List<PrimeEntryMarshal>();

        public static void Enter(string[] args)
        {
            Console.WriteLine("Entered via Bootstrap.");

            //Process.Start(filename, string.Join(" ", args));
            //return;

#if DEBUG
            args = @"publish -c [src]\instance\prime-client.config -p [src]\instance\prime_main_catalogue.config".Split(' ');
#endif
            Parser.Default.ParseArguments<PrimeBootOptions.Start, PrimeBootOptions.Publish>(args).MapResult(
                (PrimeBootOptions.Start opts) => Run(opts),
                (PrimeBootOptions.Publish pub) => Pub(pub),
                errs => 1);

        }

        private static int Pub(PrimeBootOptions.Publish options)
        {
            Console.WriteLine("Prime Publisher starting.");

            options.PubConfigPath = options.PubConfigPath.ResolveSpecial();

            var c = new PrimeContext(options.ConfigPath) {L = new ConsoleLogger()};
            var prime = new PrimeInstance(c);
            prime.Start();
            //CatalogueBootEntry.Publish(prime, options);
            prime.Stop();
            return -1;
        }

        private static int Run(PrimeBootOptions.Start options)
        {
            Console.WriteLine("Prime Bootstrapper: Entering..");

            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                CurrentDomain_ProcessExit();
            };

            Console.CancelKeyPress += Console_CancelKeyPress;

            var entry = new PrimeEntryMarshal();
            Entries.Add(entry);
            var task = Task.Factory.StartNew(() => entry.Enter(options));

            Task.WaitAll(task);
            return -1;
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
