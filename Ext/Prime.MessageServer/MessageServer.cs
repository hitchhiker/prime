using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public class MessageServer
    {
        public readonly ServerContext ServerContext;
        public readonly IMessenger M;
        public readonly ILogger L;

        private readonly List<IMessageServerExtension> _extensions = new List<IMessageServerExtension>();
        private readonly MessageTypedSender _helper;
        public readonly MessageTypeNameSerializationBinder TypeBinder;

        public MessageServer(ServerContext serverContext)
        {
            ServerContext = serverContext;
            M = ServerContext.M;
            L = ServerContext.L;
            _helper =  new MessageTypedSender(M);
            TypeBinder = new MessageTypeNameSerializationBinder(serverContext);
        }

        public void Start()
        {
            LoadExtensions();
            L.Log("Starting message servers");
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
            _extensions.AddRange(ServerContext.Types.ImplementInstances<IMessageServerExtension>());
        }

        private void ReceiveMessage(ExternalMessage m)
        {
            Task.Run(delegate
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