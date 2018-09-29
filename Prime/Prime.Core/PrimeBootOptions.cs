

using CommandLine;

namespace Prime.Core
{
    public class PrimeBootOptions
    {
        [Verb("start", HelpText = "Starts an instance of prime. Type 'start --help' for help.")]
        public class Start
        {
            [Option('c', "config", Required = true, HelpText = "Path to the prime config file.")]
            public string ConfigPath { get; set; }

            [Option("appext", Required = false, HelpText = "Check application directory during extension loading")]
            public bool AppExts { get; set; }
        }

        [Verb("publish", HelpText = "Publishing application. Type 'publish --help' for help.")]
        public class Publish
        {
            [Option('c', "config", Required = true, HelpText = "Path to the prime config file.")]
            public string ConfigPath { get; set; }

            [Option('p', "pubconfig", Required = true, HelpText = "Path to the publisher config file.")]
            public string PubConfigPath { get; set; }

            [Option('h', "hash", Required = false, HelpText = "Content hash of the catalogue index file.")]
            public string HashUri { get; set; }
        }
    }
}