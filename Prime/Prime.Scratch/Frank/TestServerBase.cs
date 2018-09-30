using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Scratch
{
    public abstract class TestServerBase
    {
        protected readonly PrimeContext S;
        protected readonly IMessenger M;
        protected readonly ILogger L;
        protected readonly Core.PrimeInstance P;

        protected TestServerBase(Core.PrimeInstance server)
        {
            P = server;
            S = server.C;
            M = S.M;
            L = S.L;
        }

        public abstract void Go();
    }
}