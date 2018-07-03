﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
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
        
        private readonly ConcurrentBag<IdentifiedClient> _connectedClients = new ConcurrentBag<IdentifiedClient>();
        
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
                connectedClient.Dispose();

            Log("stopped.");
        }

        private ExternalMessage UnpackResponse(string response, IdentifiedClient sender)
        {
            return !(_dataProvider.Deserialize(response) is BaseTransportMessage message) ? null : new ExternalMessage(sender?.Id, message);
        }

        private void WaitForClient()
        {
            if(!_stoppedRequested)
                _listener.BeginAcceptTcpClient(ClientAcceptedCallback, null);
        }
        
        private string ReceiveData(Stream stream, int receiveBufferSize = 1024)
        {
            var buffer = new byte[receiveBufferSize];
            if (stream.CanRead)
                stream.Read(buffer, 0, buffer.Length);

            return buffer.GetString();
        }

        private void ClientAcceptedCallback(IAsyncResult ar)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var connectedClient = _stoppedRequested ? new IdentifiedClient(null) : new IdentifiedClient(_listener?.EndAcceptTcpClient(ar)))
                    {
                        if (_listener == null || connectedClient.Id.IsNullOrEmpty())
                            return;

                        _connectedClients.Add(connectedClient);

                        using (var stream = connectedClient.TcpClient.GetStream())
                        {
                            while (connectedClient.TcpClient.Connected)
                            {
                                if (_stoppedRequested)
                                    return;

                                var data = ReceiveData(stream);

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
                    }
                }
                catch (Exception e)
                {
                    if (!_stoppedRequested)
                        ExceptionOccurred?.Invoke(this, e);
                }
            });

            Log("client connected");
            WaitForClient();
        }

        public void Send<T>(IdentifiedClient identifiedClient, T data)
        {
            if (identifiedClient == null)
                foreach (var c in _connectedClients)
                    TcpSendInternal(c, data);
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

        public IdentifiedClient GetClient(ObjectId clientId)
        {
            return _connectedClients.FirstOrDefault(x => x.Id == clientId);
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

