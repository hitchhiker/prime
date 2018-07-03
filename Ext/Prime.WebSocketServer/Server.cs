using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;
using Prime.Core.Testing;
using Prime.MessagingServer.Data;
using Prime.WebSocketServer.Transport;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer
{
    internal class Server
    {
        private readonly WebSocketServerContext _context;

        private readonly WebSocketSharp.Server.WebSocketServer _webSocketServer;
        private readonly CommonJsonDataProvider _commonJsonDataProvider;

        public readonly ILogger L;

        private readonly ConcurrentDictionary<ObjectId, WsRootHandler> _clientsHandlers = new ConcurrentDictionary<ObjectId, WsRootHandler>();

        public Server(WebSocketServerContext context)
        {
            _context = context;
            _commonJsonDataProvider = new CommonJsonDataProvider(_context.MessageServer);
            L = context.MessageServer.L;

            _webSocketServer = CreateWebSocketServer(context);
        }

        private WebSocketSharp.Server.WebSocketServer CreateWebSocketServer(WebSocketServerContext context)
        {
            var wsServer = new WebSocketSharp.Server.WebSocketServer(context.IpAddress, context.Port, false);
            wsServer.AddWebSocketService<WsRootHandler>(WsRootHandler.ServicePath, (x) =>
            {
                x.M = context.MessageServer.M;
                x.L = context.MessageServer.L;
                x.MessageServer = context.MessageServer;
                x.DataProvider = _commonJsonDataProvider;

                x.OnClientDisconnected = (sender, args) =>
                {
                    if (!_clientsHandlers.TryRemove(x.SessionId, out x))
                        throw new InvalidOperationException($"Client {x.SessionId} is not registered and cannot be deleted.");
                };

                x.OnClientConnected = (sender, args) =>
                {
                    if (!_clientsHandlers.TryAdd(x.SessionId, x))
                        throw new InvalidOperationException($"Client {x.SessionId} has already been connected to WebSocket server.");
                };
            });
            wsServer.AddWebSocketService<WsEchoHandler>(WsEchoHandler.ServicePath, (x) =>
            {
                x.L = context.MessageServer.L;
            });

            return wsServer;
        }

        public void Start()
        {
            _webSocketServer.Start();
            L.Log($"{WebSocketServerContext.LogServerName}: started on {_webSocketServer.Address}:{_webSocketServer.Port}.");
        }

        public void Stop()
        {
            _webSocketServer.RemoveWebSocketService("/");
            _webSocketServer.Stop();
            L.Log($"{WebSocketServerContext.LogServerName}: stopped.");
        }

        public void Send<T>(T message) where T : BaseTransportMessage
        {
            if(message.IsRemote)
                return;

            L.Log($"{WebSocketServerContext.LogServerName}: sending message...");

            var data = _commonJsonDataProvider.Serialize(message);

            if (!_clientsHandlers.TryGetValue(message.SessionId, out var client))
                throw new InvalidOperationException($"Unable to send data to client {message.SessionId} because it's not registered on WebSocket server.");

            // Sends message to client with specified SessionId.
            client.SendToCurrentClient(data.ToString());
        }
    }
}
