using Prime.Utility;

namespace Prime.Core
{
    public class ApiContext
    {
        public readonly Logger L;

        public ApiContext(Logger logger)
        {
            L = logger;
        }
    }
}