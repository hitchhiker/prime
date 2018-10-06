using CommandLine;
using Prime.Core;

namespace Prime.Radiant
{
    [Verb("ipfs", HelpText = "IPFS management.")]
    public class IpfsWinArguments
    {
        [Option("daemon", Required = false, HelpText = "Start IPFS daemon")]
        public bool DoAll { get; set; }
    }
}