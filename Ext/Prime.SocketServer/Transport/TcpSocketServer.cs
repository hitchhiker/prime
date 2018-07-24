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
    /// <summary>
    /// Inspired by https://codereview.stackexchange.com/questions/5306/tcp-socket-server
    /// </summary>
    public class TcpSocketServer
    {
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private TcpListener _listener;


        private readonly ConcurrentDictionary<ObjectId, IdentifiedClient> _connectedClients =
            new ConcurrentDictionary<ObjectId, IdentifiedClient>();

        private readonly SocketServiceProvider _serviceProvider;

        private bool _stoppedRequested;
        private int _socketPendingQueueSize = 100;
        private readonly object _lock = new object();

        public TcpSocketServer(SocketServiceProvider serviceProvider, IMessenger messenger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceProvider.TcpSocketServer = this; // TODO: AY: Frank is it okay to initialize property of object with 'this' inside constructor?
        }

        public void Start(IPAddress address, short port)
        {
            try
            {
                _connectedClients.Clear();

                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                var endpoint = new IPEndPoint(address, port);
                _socket.Bind(endpoint);
                _socket.Listen(_socketPendingQueueSize);
                _socket.BeginAccept(ClientConnectedHandler, null);

                _serviceProvider.OnServerStarted(endpoint);
            }
            catch (SocketException e)
            {
                CallOnException(e);
            }
        }

        private void ClientConnectedHandler(IAsyncResult ar)
        {
            IdentifiedClient identifiedClient = null;

            lock (_lock)
            {
                if (_socket == null)
                    return;

                // Check number of clients.
                if (_connectedClients.Count + 1 >= _socketPendingQueueSize)
                {
                    CallOnException(new InvalidOperationException("Maximum connected clients count is reached"));
                    return;
                }

                try
                {
                    var clientSocket = _socket.EndAccept(ar);
                    identifiedClient = new IdentifiedClient(clientSocket);

                    // Add client.
                    if (!_connectedClients.TryAdd(identifiedClient.Id, identifiedClient))
                        CallOnException(new InvalidOperationException($"Unable to add new client '{identifiedClient.Id}' to list of connected clients."));

                    _serviceProvider.OnClientConnected(identifiedClient.Id);

                    // Run receive thread.
                    Task.Run(() =>
                    {
                        var state = new ReceiveState(identifiedClient, (SocketServiceProvider)_serviceProvider.Clone());
                        clientSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, DataReceivedCallback, state);
                    });

                    // Wait for next client to connect.
                    _socket.BeginAccept(ClientConnectedHandler, null);
                }
                catch (SocketException e)
                {
                    CallOnException(e);

                    if (identifiedClient?.Id != null)
                        DestroyClient(identifiedClient.Id);
                }
            }
        }

        private void DataReceivedCallback(IAsyncResult ar)
        {
            var state = (ReceiveState)ar.AsyncState;
            var clientSocket = state.IdentifiedClient.ClientSocket;

            try
            {
                var bytesCount = clientSocket.EndReceive(ar);
                if (bytesCount <= 0) return;

                var clonedData = new byte[state.Buffer.Length];
                Buffer.BlockCopy(state.Buffer, 0, clonedData, 0, clonedData.Length * sizeof(byte));

                state.ServiceProvider.OnDataReceived(state.IdentifiedClient.Id, clonedData);

                clientSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    DataReceivedCallback, state);
            }
            catch (SocketException e)
            {
                CallOnException(e);
                DestroyClient(state.IdentifiedClient.Id);
            }
        }

        /// <summary>
        /// Calls OnErrorOccured with type of event set to Error. 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="clientId"></param>
        private void CallOnException(Exception e, ObjectId clientId = null)
        {
            _serviceProvider.OnErrorOccurred(e, clientId);
        }

        /// <summary>
        /// Returns connected client by specified <paramref name="clientId"/> or null if it's not connected or doesn't exist.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private IdentifiedClient GetClient(ObjectId clientId)
        {
            if (clientId == null)
                return null;

            return !_connectedClients.TryGetValue(clientId, out var client) ? null : client;
        }

        /// <summary>
        /// Finds client with specified id and then destroys it.
        /// </summary>
        /// <param name="clientId"></param>
        private void DestroyClient(ObjectId clientId)
        {
            // Get client and check if is in connected clients list.
            var connectedClient = GetClient(clientId);
            if (connectedClient == null)
                _serviceProvider.OnErrorOccurred(new InvalidOperationException("Unable to destroy client because it is not present in list of connected clients."), clientId);

            // Disconnect client.
            connectedClient?.Dispose();

            // Remove client from list of connected clients.
            if (!_connectedClients.TryRemove(clientId, out var deletedClient))
                _serviceProvider.OnErrorOccurred(new InvalidOperationException("Unable to destroy client."), clientId);

            _serviceProvider.OnClientDisconnected(clientId);
        }

        public void Stop()
        {
            _stoppedRequested = true;

            _socket.Disconnect(false);
            _socket.Dispose();

            foreach (var connectedClient in _connectedClients)
                connectedClient.Value.Dispose();

            _connectedClients.Clear();

            _serviceProvider.OnServerStopped();
        }

        /// <summary>
        /// Sends message to connected client.
        /// If  <paramref name="clientId"/> is null, sends to all connected clients.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="data"></param>
        public void Send(ObjectId clientId, byte[] data)
        {
            if (clientId == null)
                foreach (var c in _connectedClients)
                    SendDirect(c.Value, data);
            else
                SendDirect(clientId, data);
        }

        private void SendDirect(IdentifiedClient client, byte[] data)
        {
            if (client == null)
            {
                CallOnException(new InvalidOperationException("Client is not connected to socket server."));
                return;
            }

            try
            {
                client.ClientSocket.Send(data);
            }
            catch (SocketException e)
            {
                CallOnException(e, client.Id);
            }
        }

        private void SendDirect(ObjectId clientId, byte[] data)
        {
            var client = GetClient(clientId);
            SendDirect(client, data);
        }

        private bool IsSocketConnected(Socket socket)
        {
            bool part1 = socket.Poll(100, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);

            return !(part1 && part2);
        }
    }
}