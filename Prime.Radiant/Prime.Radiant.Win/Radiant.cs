using Prime.Utility;

namespace Prime.Radiant
{
    public class Radiant
    {
        public static string DnsVersion = "v2";

        public readonly ILogger L;

        public Radiant(ILogger logger)
        {
            L = logger;
            IpfsDaemon = new IpFsDaemon(logger);
            IpFsApi = new IpFsApi(IpfsDaemon);
        }

        public IpFsDaemon IpfsDaemon { get; }

        public IpFsApi IpFsApi { get; }
    }
}