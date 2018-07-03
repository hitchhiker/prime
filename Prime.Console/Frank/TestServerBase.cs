using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Console.Frank
{
    public abstract class TestServerBase
    {
        protected readonly ServerContext S;
        protected readonly IMessenger M;
        protected readonly ILogger L;
        protected readonly Core.PrimeInstance P;

        protected TestServerBase(Core.PrimeInstance server)
        {
            P = server;
            S = server.Context;
            M = S.M;
            L = S.L;
        }

        public abstract void Go();
    }
}