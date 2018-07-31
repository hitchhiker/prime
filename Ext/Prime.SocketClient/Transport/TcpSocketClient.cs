using System;
using System.Net;
using System.Net.Sockets;
using Prime.Base.Misc.Utils;
using Prime.Core;
using Prime.MessagingServer.Data;

namespace Prime.SocketClient.Transport
{
    /// <summary>
    /// Local test implementation of TCP socket client. Should not be used for production.
    /// TODO: move to new Prime.SocketClient project?
    /// </summary>
    public class TcpSocketClient
    {
        private readonly Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly CommonJsonDataProvider _dataProvider;
        private readonly MessagingServer.Server _messagingServer;

        public TcpSocketClient(MessagingServer.Server messagingServer)
        {
            _dataProvider = new CommonJsonDataProvider(messagingServer);
            _messagingServer = messagingServer;
        }

        public void Connect(IPEndPoint ipEndPoint)
        {
            _clientSocket.Connect(ipEndPoint);
            L.Log($"Connected to socket server {ipEndPoint}.");

            var state = new ReceiveState();
            _clientSocket.BeginReceive(state.Buffer, 0, sizeof(UInt32), SocketFlags.None, ReceivedCallback, state);
        }

        private void ReceivedCallback(IAsyncResult ar)
        {
            var state = (ReceiveState)ar.AsyncState;

            lock (state)
            {
                var bytesRead = _clientSocket.EndReceive(ar);
                if (bytesRead <= 0) return;

                // BUG: duplicated code (TcpSocketServer.DataReceivedCallback).
                if (!state.ExpectedMessageSize.HasValue)
                {
                    // Size is not received.
                    if (bytesRead != sizeof(UInt32))
                        throw new InvalidOperationException(
                            $"The length of buffer size byte array is not equal to {sizeof(UInt32)}.");

                    var buffer = state.Buffer;

                    var messageSize = ByteUtils.ExtractDataSize(buffer);
                    if (messageSize == 0)
                        throw new InvalidOperationException("Size of message equals to 0.");

                    Array.Clear(buffer, 0, buffer.Length);

                    state.ExpectedMessageSize = messageSize;
                    _clientSocket.BeginReceive(buffer, 0, (int)messageSize, SocketFlags.None,
                        ReceivedCallback, state);
                }
                else
                {
                    var buffer = state.Buffer;
                    var receivedBytes = new byte[buffer.Length];
                    Buffer.BlockCopy(buffer, 0, receivedBytes, 0, receivedBytes.Length);

                    ProcessReceivedData(receivedBytes);

                    state.ExpectedMessageSize = null;
                    Array.Clear(buffer, 0, buffer.Length);
                    _clientSocket.BeginReceive(buffer, 0, sizeof(UInt32), SocketFlags.None, ReceivedCallback, state);
                }
            }
        }

        private void ProcessReceivedData(byte[] receivedBytes)
        {
            var receivedString = receivedBytes.DecodeString();
            var m = UnpackResponse(receivedString);

            if (m != null)
                _messagingServer.M.SendAsync(m);
        }

        private ExternalMessage UnpackResponse(string response)
        {
            var deserializedObject = _dataProvider.Deserialize(response);
            return !(deserializedObject is BaseTransportMessage message)
                ? null
                : new ExternalMessage(message.SessionId, message);
        }

        public void Disconnect()
        {
            if (_clientSocket.Connected)
            {
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Disconnect(true);
            }
        }

        public void Send(BaseTransportMessage message)
        {
            var data = _dataProvider.Serialize(message).ToString();
            var dataBytes = data.GetBytes();

            var prefixedDataBytes = ByteUtils.PrefixBufferSize(dataBytes);
            
            _clientSocket.SendAsync(new ArraySegment<byte>(prefixedDataBytes), SocketFlags.None);
        }

        public ILogger L { get; set; }
    }
}
