using CommandLine;
using Newtonsoft.Json;

namespace Prime.Core
{
    public class PackageManagerArguments
    {
        public class BaseArguments
        {
            [Option('a', "all", Required = false, HelpText = "Do all catalogues (default)")]
            public bool DoAll { get; set; } = true;

            [Option("catalogue", Required = false, HelpText = "Only catalogue specified by public key.")]
            public string CataloguePubKey { get; set; }
        }

        [Verb("compile", HelpText = "Compile package catalogue source projects")]
        public class CompileArguments
        {
            [Option('p', "packageConfig", Required = true, HelpText = "Path to the package config file.")]
            public string PackageConfigPath { get; set; }
        }

        [Verb("build", HelpText = "Build the packages catalogue")]
        public class BuildArguments : BaseArguments
        {
            [Option('p', "pubconfig", Required = true, HelpText = "Path to the publisher config file.")]
            public string PubConfigPath { get; set; }
        }
    }
}