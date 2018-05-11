using Prime.Base;
using Prime.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.SocketServer
{
    public class TcpServer
    {
        private readonly SocketServer _server;
        private TcpListener _listener;
        private readonly ConcurrentBag<IdentifiedClient> _connectedClients = new ConcurrentBag<IdentifiedClient>();
        private readonly JsonDataProvider _dataProvider;
        public readonly ILogger L;
        public readonly IMessenger M;

        public TcpServer(SocketServer server, IMessenger messenger = null)
        {
            _server = server;
            L = _server?.Context?.MessageServer?.L ?? new NullLogger();
            M = messenger ?? _server.Context.MessageServer.M;
            _dataProvider = new JsonDataProvider(server.Context.MessageServer);
        }

        public TcpServer(MessageServer server, IMessenger messenger = null)
        {
            _server = null;
            L = _server?.Context?.MessageServer?.L ?? new NullLogger();
            M = messenger ?? _server.Context.MessageServer.M;
            _dataProvider = new JsonDataProvider(server);
        }

        public void Start(IPAddress address, short port)
        {
            _connectedClients.Clear();
            _listener = new TcpListener(address, port);
            _listener.Start();

            Log("TCP socket server started.");
            WaitForClient();
        }

        public void Stop()
        {
            _listener.Stop();

            foreach (var connectedClient in _connectedClients)
                connectedClient.Dispose();

            Log("TCP socket server stopped.");
        }

        public ExternalMessage UnpackResponse(object response, IdentifiedClient sender)
        {
            return !(_dataProvider.Deserialize(response) is BaseTransportMessage message) ? null : new ExternalMessage(sender?.Id, message);
        }

        private void WaitForClient()
        {
            _listener.BeginAcceptTcpClient(ClientAcceptedCallback, null);
        }

        private void ClientAcceptedCallback(IAsyncResult ar)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var connectedClient = new IdentifiedClient(_listener.EndAcceptTcpClient(ar)))
                    {
                        _connectedClients.Add(connectedClient);

                        using (var stream = connectedClient.TcpClient.GetStream())
                        {
                            while (connectedClient.TcpClient.Connected)
                            {
                                var data = _dataProvider.ReceiveData(stream);

                                try
                                {
                                    var m = UnpackResponse(data, connectedClient);

                                    if (m != null)
                                        M.SendAsync(m);
                                }
                                catch (Exception e)
                                {
                                    L.Error("Exception in TcpServer: " + e.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionOccurred?.Invoke(this, e);
                }
            });

            Log("Client connected");
            WaitForClient();
        }

        public void Send<T>(IdentifiedClient identifiedClient, T data)
        {
            if (identifiedClient==null)
                foreach (var c in _connectedClients)
                    _dataProvider.SendData(c, data);
            else
                _dataProvider.SendData(identifiedClient, data);

        }

        public IdentifiedClient GetClient(ObjectId clientId)
        {
            return _connectedClients.FirstOrDefault(x => x.Id == clientId);
        }

        public event EventHandler<Exception> ExceptionOccurred;

        private void Log(string text)
        {
            L.Log($"({typeof(TcpServer).Name}) : {text}");
        }
    }
}

