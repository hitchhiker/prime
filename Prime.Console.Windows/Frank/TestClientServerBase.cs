using Prime.Core;

namespace Prime.ConsoleApp.Tests.Frank
{
    public abstract class TestClientServerBase
    {
        protected readonly ServerContext S;
        protected readonly ClientContext C;

        protected TestClientServerBase(ServerContext server, ClientContext c)
        {
            S = server;
            C = c;
        }

        public abstract void Go();
    }
}