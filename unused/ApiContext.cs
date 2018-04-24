using Prime.Utility;

namespace Prime.Common
{
    public class ApiContext
    {
        public readonly ILogger L;

        public ApiContext(ILogger logger)
        {
            L = logger;
        }
    }
}