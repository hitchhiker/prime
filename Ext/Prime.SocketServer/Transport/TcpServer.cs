using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer.Data;

namespace Prime.SocketServer.Transport
{
    public class TcpServer
    {
        private readonly Server _server;
        private TcpListener _listener;
        private readonly CommonJsonDataProvider _dataProvider;

        private readonly ConcurrentDictionary<ObjectId, IdentifiedClient> _connectedClients =
            new ConcurrentDictionary<ObjectId, IdentifiedClient>();

        public readonly ILogger L;
        public readonly IMessenger M;

        private bool _stoppedRequested;

        public TcpServer(Server server, IMessenger messenger = null)
        {
            _server = server;

            L = _server?.Context?.MessagingServer?.L ?? new NullLogger();
            M = messenger ?? _server?.Context?.MessagingServer?.M;

            _dataProvider = new CommonJsonDataProvider(server.Context.MessagingServer);
        }

        public void Start(IPAddress address, short port)
        {
            _stoppedRequested = false;

            _connectedClients.Clear();
            _listener = new TcpListener(address, port);
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _listener.Start();

            Log($"started on {_listener.LocalEndpoint}.");
            WaitForClient();
        }

        public void Stop()
        {
            _stoppedRequested = true;

            _listener.Stop();
            _listener = null;

            foreach (var connectedClient in _connectedClients)
                connectedClient.Value.Dispose();

            _connectedClients.Clear();

            Log("stopped.");
        }

        private ExternalMessage UnpackResponse(string response, IdentifiedClient sender)
        {
            return !(_dataProvider.Deserialize(response) is BaseTransportMessage message)
                ? null
                : new ExternalMessage(sender?.Id, message);
        }

        private void WaitForClient()
        {
            if (!_stoppedRequested)
                _listener.BeginAcceptTcpClient(ClientAcceptedCallback, null);
        }

        private string ReceiveData(NetworkStream stream, int receiveBufferSize = 1024)
        {
            var buffer = new byte[receiveBufferSize];
            Console.WriteLine($"Mem: {Process.GetCurrentProcess().WorkingSet64}");

            if (stream.CanRead && stream.DataAvailable)
                stream.Read(buffer, 0, buffer.Length);

            return buffer.GetString();
        }

        private void ClientAcceptedCallback(IAsyncResult ar)
        {
            // TODO: test with normal clients. 'nc' client doesn't seem to call disconnect code, so clients are always collected in dictionary without deleting.
            // TODO: maybe we need to implement periodical clients cleanup when they are not responding to messeges.
            // TODO: or limiting number of connections?
            Task.Run(() =>
            {
                try
                {
                    using (var connectedClient = _stoppedRequested
                        ? new IdentifiedClient(null)
                        : new IdentifiedClient(_listener?.EndAcceptTcpClient(ar)))
                    {
                        if (_listener == null || connectedClient.Id.IsNullOrEmpty())
                            return;

                        if (!_connectedClients.TryAdd(connectedClient.Id, connectedClient))
                            Error($"unable to add new socket client '{connectedClient.Id}' to concurrent dictionary.");

                        Log($"client '{connectedClient.Id}' connected.");

                        using (var stream = connectedClient.TcpClient.GetStream())
                        {
                            var buffer = new byte[1024];

                            //stream.Read(buffer, 0, buffer.Length);

                            var br = stream.BeginRead(buffer, 0, buffer.Length, ClientReadCallback, null);

                            while (connectedClient.TcpClient.Client.Connected)
                            {
                                if (_stoppedRequested)
                                    return;

                                string data = null;
                                if (stream.CanRead && stream.DataAvailable)
                                {
                                    stream.Read(buffer, 0, buffer.Length);
                                    data = buffer.GetString();
                                }

                                if (string.IsNullOrEmpty(data))
                                    continue;

                                try
                                {
                                    var m = UnpackResponse(data, connectedClient);

                                    if (m != null)
                                        M.SendAsync(m);
                                }
                                catch (Exception e)
                                {
                                    Error(e.Message);
                                }
                            }
                        }

                        if (_connectedClients.TryRemove(connectedClient.Id, out var deletedClient))
                            Log($"client {connectedClient.Id} disconnected.");
                        else 
                            Error($"unable to delete connected client '{connectedClient.Id}'.");
                    }
                }
                catch (Exception e)
                {
                    if (!_stoppedRequested)
                        ExceptionOccurred?.Invoke(this, e);
                }
            });

            WaitForClient();
        }

        private void ClientReadCallback(IAsyncResult ar)
        {
            
        }

        /// <summary>
        /// Sends message to connected client.
        /// If  <paramref name="identifiedClient"/> is null sends to all connected clients.
        /// </summary>
        /// <param name="identifiedClient"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void Send<T>(IdentifiedClient identifiedClient, T data)
        {
            if (identifiedClient == null)
                foreach (var c in _connectedClients)
                    TcpSendInternal(c.Value, data);
            else
                TcpSendInternal(identifiedClient, data);
        }

        private void TcpSendInternal<T>(IdentifiedClient identifiedClient, T data)
        {
            if (!identifiedClient.TcpClient.Connected)
            {
                Warn("Attempted to write to closed TcpClient.");
                return;
            }

            var dataString = _dataProvider.Serialize(data).ToString();
            var dataBytes = dataString.GetBytes();

            identifiedClient.TcpClient.GetStream().Write(dataBytes, 0, dataBytes.Length);
        }

        /// <summary>
        /// Returns connected client by specified <paramref name="clientId"/> or null if it's not connected or doesn't exist.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public IdentifiedClient GetClient(ObjectId clientId)
        {
            if (!_connectedClients.TryGetValue(clientId, out var client))
            {
                Warn($"unable to get client '{clientId}'");
            }

            return client;
        }

        public event EventHandler<Exception> ExceptionOccurred;

        private void Log(string text, LoggingLevel loggingLevel = LoggingLevel.Status)
        {
            L.Log($"{SocketServerContext.LogServerName}: {text}", loggingLevel);
        }

        private void Error(string text)
        {
            Log(text, LoggingLevel.Error);
        }

        private void Warn(string text)
        {
            Log(text, LoggingLevel.Warning);
        }
    }
}