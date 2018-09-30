using System;
using System.Diagnostics;
using CommandLine;
using Prime.Core;

namespace Prime.IPNS.Resource
{
    public class CommandLineOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to the prime config file.")]
        public string ConfigPath { get; set; }

        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Run(new CommandLineOptions()
                {
                    ConfigPath = "..\\..\\..\\..\\..\\..\\instance\\prime-client.config"
                });
                Console.ReadLine();
            }
            else
                Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(Run);
        }

        static void Run(CommandLineOptions opts)
        {
            //var pctx = new PrimeInstance
        }
    }
}
