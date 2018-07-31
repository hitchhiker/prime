﻿using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;
using Prime.KeysManager;
using Prime.MessagingServer;
using Prime.Settings;

namespace Prime
{
    public class PrimeWsServer
    {
        private readonly ServerContext _serverContext;
        private readonly MessagingServer.Server _messagingServer;

        public PrimeWsServer(ServerContext serverContext, Server messagingServer)
        {
            _serverContext = serverContext;
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
