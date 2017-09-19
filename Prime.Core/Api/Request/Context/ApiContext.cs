using Prime.Utility;

namespace Prime.Core
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