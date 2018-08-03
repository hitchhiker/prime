using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using Prime.Base;
using Prime.Base.Misc.Utils;
using Prime.MessagingServer.Data;
using Timer = System.Timers.Timer;

namespace Prime.SocketServer.Transport
{
    /// <summary>
    /// Inspired by https://codereview.stackexchange.com/questions/5306/tcp-socket-server
    /// </summary>
    public class TcpSocketServer
    {
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly ConcurrentDictionary<ObjectId, IdentifiedClient> _connectedClients =
            new ConcurrentDictionary<ObjectId, IdentifiedClient>();

        private readonly SocketServiceProvider _serviceProvider;
        private readonly Timer _pollingTimer = new Timer();

        private int _socketPendingQueueSize = 100;
        private readonly object _lock = new object();

        // TODO: AY: implement 'soft' stopping.
        private bool _stopRequested;

        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan DontPollPeriod { get; set; } = TimeSpan.FromSeconds(5);

        public TcpSocketServer(SocketServiceProvider serviceProvider, IMessenger messenger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceProvider.TcpSocketServer =
                this; // TODO: AY: Frank is it okay to initialize property of object with 'this' inside constructor?
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

                StartPolling();

                _serviceProvider.OnServerStarted(endpoint);
            }
            catch (SocketException e)
            {
                CallOnException(e);
            }
        }

        public void StartPolling()
        {
            _pollingTimer.Stop();
            _pollingTimer.Interval = PollingInterval.TotalMilliseconds;
            _pollingTimer.Elapsed += PollingTimerOnElapsed;
            _pollingTimer.Start();
        }

        private void PollingTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Parallel.ForEach(_connectedClients.Where(Predicate), pair =>
            {
                var isAlive = IsSocketConnected(pair.Value.ClientSocket);
                Console.WriteLine(
                    $"{Thread.CurrentThread.ManagedThreadId}: '{pair.Key}' is {(isAlive ? "alive" : "dead")}.");
                if (isAlive)
                    pair.Value.UpdateLastRead();
                else
                {
                    DestroyClient(pair.Key);
                }
            });

            bool Predicate(KeyValuePair<ObjectId, IdentifiedClient> pair)
            {
                if (!pair.Value.LastActivityUtc.HasValue)
                    return false;

                return (DateTime.UtcNow - pair.Value.LastActivityUtc.Value) > DontPollPeriod;
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
                    CallOnException(new InvalidOperationException("Maximum connected clients count is reached."));
                    return;
                }

                try
                {
                    var clientSocket = _socket.EndAccept(ar);
                    identifiedClient = new IdentifiedClient(clientSocket);

                    // Add client.
                    if (!_connectedClients.TryAdd(identifiedClient.Id, identifiedClient))
                        CallOnException(new InvalidOperationException(
                            $"Unable to add new client '{identifiedClient.Id}' to list of connected clients."));

                    _serviceProvider.OnClientConnected(identifiedClient.Id);

                    // Run receive thread.
                    Task.Run(() =>
                    {
                        var state = new ReceiveState(identifiedClient,
                            (SocketServiceProvider) _serviceProvider.Clone());
                        clientSocket.BeginReceive(state.Buffer, 0, sizeof(UInt32), SocketFlags.None,
                            DataReceivedCallback, state);
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
            var state = (ReceiveState) ar.AsyncState;
            var clientSocket = state.IdentifiedClient.ClientSocket;

            lock (state)
            {
                try
                {
                    var bytesCount = clientSocket.EndReceive(ar);
                    if (bytesCount <= 0) return;

                    if (!state.ExpectedMessageSize.HasValue)
                    {
                        // Size is not received.
                        if (bytesCount != sizeof(UInt32))
                            throw new InvalidOperationException(
                                $"The length of buffer size byte array is not equal to {sizeof(UInt32)}.");

                        var messageSize = ByteUtils.ExtractDataSize(state.Buffer);
                        state.ExpectedMessageSize = messageSize;

                        Array.Clear(state.Buffer, 0, state.Buffer.Length);
                        clientSocket.BeginReceive(state.Buffer, 0, (int)messageSize, SocketFlags.None,
                            DataReceivedCallback, state);
                    }
                    else
                    {
                        var clonedData = new byte[state.Buffer.Length];
                        Buffer.BlockCopy(state.Buffer, 0, clonedData, 0, clonedData.Length);

                        state.IdentifiedClient.UpdateLastRead();
                        state.ServiceProvider.OnDataReceived(state.IdentifiedClient.Id, clonedData);

                        Array.Clear(state.Buffer, 0, state.Buffer.Length);

                        state.ExpectedMessageSize = null;
                        clientSocket.BeginReceive(state.Buffer, 0, sizeof(UInt32), SocketFlags.None,
                            DataReceivedCallback, state);
                    }
                }
                catch (SocketException e)
                {
                    CallOnException(e);
                    DestroyClient(state.IdentifiedClient.Id);
                }
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
            lock (_lock)
            {
                // Get client and check if is in connected clients list.
                var connectedClient = GetClient(clientId);
                if (connectedClient == null)
                    _serviceProvider.OnErrorOccurred(
                        new InvalidOperationException(
                            "Unable to destroy client because it is not present in list of connected clients."),
                        clientId);

                // Disconnect client.
                connectedClient?.Dispose();

                // Remove client from list of connected clients.
                if (!_connectedClients.TryRemove(clientId, out var deletedClient))
                    _serviceProvider.OnErrorOccurred(
                        new InvalidOperationException("Unable to remove client from list of connected clients."),
                        clientId);

                _serviceProvider.OnClientDisconnected(clientId);
            }
        }

        public void ForceStop()
        {
            lock (_lock)
            {
                _stopRequested = true;

                _socket.Disconnect(false);
                _socket.Dispose();

                foreach (var connectedClient in _connectedClients)
                    connectedClient.Value.Dispose();

                _connectedClients.Clear();

                _serviceProvider.OnServerStopped();
            }
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
                lock (_lock)
                {
                    var prefixedData = ByteUtils.PrefixBufferSize(data);

                    client.ClientSocket.Send(prefixedData);
                    client.UpdateLastWrite();
                }
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

        /// <summary>
        /// Polls socket and checks if client is still connected.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private bool IsSocketConnected(Socket socket)
        {
            bool part1 = socket.Poll(100, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);

            return !(part1 && part2);
        }
    }
}