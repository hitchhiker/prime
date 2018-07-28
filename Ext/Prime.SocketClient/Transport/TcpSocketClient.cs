using System;
using System.Net;
using System.Net.Sockets;
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

            var buffer = new byte[1024];
            _clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, buffer);
        }

        private void ReceivedCallback(IAsyncResult ar)
        {
            var bytesRead = _clientSocket.EndReceive(ar);
            var buffer = (byte[]) ar.AsyncState;

            if (bytesRead <= 0) return;

            var receivedBytes = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, receivedBytes, 0, receivedBytes.Length);

            ProcessReceivedData(receivedBytes);
            
            Array.Clear(buffer, 0, buffer.Length);
            _clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, buffer);
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
            _clientSocket.SendAsync(new ArraySegment<byte>(data.GetBytes()), SocketFlags.None);
        }

        public ILogger L { get; set; }
    }
}
