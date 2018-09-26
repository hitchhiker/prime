using Prime.Core;

namespace Prime.Console.Frank
{
    public abstract class TestClientServerBase
    {
        protected readonly PrimeContext S;
        protected readonly PrimeContext C;

        protected TestClientServerBase(PrimeContext serverContext, PrimeContext clientContext)
        {
            S = serverContext;
            C = clientContext;
        }

        public abstract void Go();
    }
}