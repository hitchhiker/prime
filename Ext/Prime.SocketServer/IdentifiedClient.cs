using System;
using System.Net.Sockets;
using Prime.Base;

namespace Prime.SocketServer
{
    public class IdentifiedClient : IDisposable
    {
        public readonly TcpClient TcpClient;
        public readonly ObjectId Id;

        public IdentifiedClient(TcpClient tcpClient)
        {
            Id = ObjectId.NewObjectId();
            TcpClient = tcpClient;
        }

        public void Dispose()
        {
            TcpClient?.Dispose();
        }
    }
}