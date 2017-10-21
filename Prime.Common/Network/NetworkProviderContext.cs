using Prime.Utility;

namespace Prime.Common
{
    public class NetworkProviderContext
    {
        public readonly ILogger L;

        public NetworkProviderContext(ILogger logger = null)
        {
            L = logger ?? Logging.I.DefaultLogger;
        }

        public virtual bool IsPublic => true;

        public bool IsPrivate => !IsPublic;
    }
}