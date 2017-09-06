using Prime.Utility;

namespace Prime.Core
{
    public class NetworkProviderContext
    {
        public readonly Logger L;

        public NetworkProviderContext(Logger logger = null)
        {
            L = logger ?? Logging.I.Common;
        }

        public virtual bool IsPublic => true;

        public bool IsPrivate => !IsPublic;
    }
}