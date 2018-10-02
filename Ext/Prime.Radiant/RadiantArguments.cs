using CommandLine;
using Prime.Core;

namespace Prime.Radiant
{
    [Verb("catalogue", HelpText = "Catalogue management.")]
    public class RadiantArguments
    {
        [Verb("update", HelpText = "Perform catalogue update.")]
        public class UpdateArguments
        {
            [Option('a', "all", Required = false, HelpText = "update all catalogues")]
            public bool DoAll { get; set; }

            [Option("catalogue", Required = false, HelpText = "Update catalogue specified by public key.")]
            public string CataloguePubKey { get; set; }
        }

        [Verb("publish", HelpText = "Perform catalogue publish.")]
        public class PublishArguments
        {
            [Option('p', "pubconfig", Required = true, HelpText = "Path to the publisher config file.")]
            public string PubConfigPath { get; set; }

            [Option('h', "hash", Required = false, HelpText = "Content hash of the catalogue index file.")]
            public string HashUri { get; set; }
        }
    }
}