using CommandLine;
using Prime.Base;

namespace Prime.NetCoreExtensionPackager
{
    public class NetCoreExtensionPackagerArguments
    {
        [Option('c', "config", Required = true, HelpText = "Path to the prime config file.")]
        public string ConfigPath { get; set; }

        [Option('e', "ext", Required = true, HelpText = "Path to the extension directory for inspection.")]
        public string ExtPath { get; set; }

        [Option("key", HelpText = "Used to force inspection of only the extension matching this hashed name key.")]
        public string ExtensionKey { get; set; }

        [Option("id", HelpText = "Used to force inspection of only the extension matching this id.")]
        public ObjectId ExtensionId { get; set; }

        [Option("nuget", HelpText = "Used to force collection of missing nuget libraries [experimental].")]
        public bool Nuget { get; set; }

        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}