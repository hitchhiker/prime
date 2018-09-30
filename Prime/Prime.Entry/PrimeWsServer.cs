using Prime.Core;
using Prime.MessagingServer;

namespace Prime
{
    public class PrimeWsServer
    {
        private readonly PrimeContext _primeContext;
        private readonly MessagingServer.Server _messagingServer;

        public PrimeWsServer(PrimeContext primeContext, Server messagingServer)
        {
            _primeContext = primeContext;
            _messagingServer = messagingServer;
        }

        public void Start()
        {
            var wsServerExtension = new WebSocketServer.ServerExtension();
            _messagingServer.Inject(wsServerExtension);
           
            //wsServerExtension.Start(_messagingServer);
            L.Log("Ws server started");
        }

        public ILogger L { get; set; }
    }
}
