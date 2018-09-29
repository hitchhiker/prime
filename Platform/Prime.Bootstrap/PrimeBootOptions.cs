using CommandLine;

namespace Prime.Core
{
    public class PrimeBootOptionsBase
    {
        [Option('c', "config", Required = true, HelpText = "Path to the prime config file.")]
        public string ConfigPath { get; set; }
    }

    public class PrimeBootOptions
    {
        [Verb("start", HelpText = "Starts an instance of prime.")]
        public class Start : PrimeBootOptionsBase
        {
            [Option("appext", Required = false, HelpText = "Check application directory during extension loading")]
            public bool AppExts { get; set; }
        }

        [Verb("publish", HelpText = "Publishing tool.")]
        public class Publish : PrimeBootOptionsBase
        {
            [Option('p', "pubconfig", Required = true, HelpText = "Path to the publisher config file.")]
            public string PubConfigPath { get; set; }

            [Option('h', "hash", Required = false, HelpText = "Content hash of the catalogue index file.")]
            public string HashUri { get; set; }
        }

        [Verb("update", HelpText = "Update catalogue.")]
        public class Update : PrimeBootOptionsBase
        {
            [Option("uri", Required = false, HelpText = "Content uri of the catalogue index file.")]
            public string HashUri { get; set; }
        }

    }
}