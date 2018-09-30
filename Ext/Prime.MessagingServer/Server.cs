using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.MessagingServer.Types;

namespace Prime.MessagingServer
{
    public class Server
    {
        public readonly PrimeContext PrimeContext;
        public readonly IMessenger M;
        public readonly ILogger L;

        private readonly List<IMessageServerExtension> _extensions = new List<IMessageServerExtension>();
        private readonly MessageTypedSender _helper;
        public readonly MessageTypeNameSerializationBinder TypeBinder;

        public Server(PrimeContext primeContext)
        {
            PrimeContext = primeContext;
            M = PrimeContext.M;
            L = PrimeContext.L;
            _helper =  new MessageTypedSender(M);
            TypeBinder = new MessageTypeNameSerializationBinder(primeContext);
            LoadExtensions();
        }

        public void Inject(IMessageServerExtension extension)
        {
            if (_extensions.Any(x => x.Id == extension.Id))
                return; //TODO: Eventually we'll allow same package on diff parameters.

            _extensions.Add(extension);
        }

        public void Start()
        {
            // LoadExtensions(); // AY: Call of this clears extensions that are loaded in constructor and using Inject().
            L.Log($"Starting message server with {_extensions.Count} extensions.");
            M.RegisterAsync<BaseTransportMessage>(this, PossibleSendMessage, true);
            M.RegisterAsync<ExternalMessage>(this, ReceiveMessage);
            _extensions.ForEach(x => x.Start(this));
        }

        public void Stop()
        {
            _extensions.ForEach(x => x.Stop());
            _extensions.Clear();
            M.UnregisterAsync(this);
            L.Log("Stopping message server");
        }

        public void LoadExtensions()
        {
            _extensions.Clear();
            _extensions.AddRange(PrimeContext.Extensions.OfType<IMessageServerExtension>());
        }

        private void ReceiveMessage(ExternalMessage m)
        {
            Task.Run(() => 
            {
                _helper.UnPackSendReceivedMessage(m);
            });
        }

        private void PossibleSendMessage(BaseTransportMessage m)
        {
            if (m.IsRemote)
                return;

            _extensions.ForEach(x => Task.Run(() => x.Send(m)));
        }
    }
}