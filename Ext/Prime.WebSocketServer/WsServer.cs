using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Prime.Core;
using Prime.Core.Testing;
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

        private WsRootHandler _rootHandlerInst;

        public WsServer(WsServerContext context)
        {
            _context = context;

            _webSocketServer = new WebSocketSharp.Server.WebSocketServer(context.IpAddress, context.Port, false);
            _webSocketServer.AddWebSocketService<WsRootHandler>(WsRootHandler.ServicePath, (x) =>
            {
                x.M = context.MessageServer.M;
                x.L = context.MessageServer.L;
                x.MessageServer = context.MessageServer;

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

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _context.MessageServer.TypeBinder
            };

            _rootHandlerInst.SendData(JsonConvert.SerializeObject(message, settings));
        }
    }
}
