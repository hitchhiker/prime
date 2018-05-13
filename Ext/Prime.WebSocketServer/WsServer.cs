using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;
using Prime.WebSocketServer.Transport;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer
{
    internal class WsServer
    {
        private readonly WsServerContext _context;

        private readonly WebSocketSharp.Server.WebSocketServer _webSocketServer;

        public readonly ILogger L;

        //public WebSocketSessionManager SessionManager;

        public WsServer(WsServerContext context)
        {
            _context = context;

            _webSocketServer = new WebSocketSharp.Server.WebSocketServer(context.IpAddress, context.Port, false);
            _webSocketServer.AddWebSocketService<WsCommonService>("/", (x) =>
            {
                x.M = context.MessageServer.M;
                x.L = context.MessageServer.L;
                //SessionManager = x.SessionManager;
            });

            L = context.MessageServer.L;
        }

        public void Start()
        {
            _webSocketServer.Start();
            L.Log($"WsServer started.");
        }

        public void Stop()
        {
            _webSocketServer.Stop();
            L.Log($"WsServer stopped.");
        }

        public void Send<T>(T message) where T : BaseTransportMessage
        {
            L.Log($"WsServer sending message...");
        }
    }
}
