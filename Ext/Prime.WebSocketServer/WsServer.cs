using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Prime.Core;
using Prime.Core.Testing;
using Prime.MessageServer.Data;
using Prime.WebSocketServer.Transport;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer
{
    internal class WsServer
    {
        private readonly WsServerContext _context;

        private readonly WebSocketSharp.Server.WebSocketServer _webSocketServer;
        private readonly JsonDataProvider _jsonDataProvider;

        public readonly ILogger L;

        private WsRootHandler _rootHandlerInst;

        public WsServer(WsServerContext context)
        {
            _context = context;
            _jsonDataProvider = new JsonDataProvider(_context.MessageServer);

            _webSocketServer = new WebSocketSharp.Server.WebSocketServer(context.IpAddress, context.Port, false);
            _webSocketServer.AddWebSocketService<WsRootHandler>(WsRootHandler.ServicePath, (x) =>
            {
                x.M = context.MessageServer.M;
                x.L = context.MessageServer.L;
                x.MessageServer = context.MessageServer;
                x.DataProvider = _jsonDataProvider;

                _rootHandlerInst = x;
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
            _webSocketServer.RemoveWebSocketService("/");
            _webSocketServer.Stop();
            L.Log($"WsServer stopped.");
        }

        public void Send<T>(T message) where T : BaseTransportMessage
        {
            L.Log($"WsServer sending message...");

            var data = _jsonDataProvider.Serialize(message);
            _rootHandlerInst.SendTo(data.ToString(), "TEST");
        }
    }
}
