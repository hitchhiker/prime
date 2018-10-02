using CommandLine;

namespace Prime.Core
{
    public class PrimeCoreArguments
    {
        public class Start
        {
            [Option("appext", Required = false, HelpText = "Check application directory during extension loading")]
            public bool AppExts { get; set; }
        }
    }
}