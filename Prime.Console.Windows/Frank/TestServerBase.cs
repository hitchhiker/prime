using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.ConsoleApp.Tests.Frank
{
    public abstract class TestServerBase
    {
        protected readonly ServerContext S;
        protected readonly IMessenger M;
        protected readonly ILogger L;

        protected TestServerBase(ServerContext server)
        {
            S = server;
            M = S.M;
            L = S.L;
        }

        public abstract void Go();
    }
}