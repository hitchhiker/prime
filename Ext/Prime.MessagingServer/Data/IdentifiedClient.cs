using System;
using System.Net.Sockets;
using Prime.Base;

namespace Prime.MessagingServer.Data
{
    public class IdentifiedClient : IDisposable
    {
        public readonly TcpClient TcpClient;
        public readonly ObjectId Id;

        public IdentifiedClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                return;

            Id = ObjectId.NewObjectId();
            TcpClient = tcpClient;
        }

        public void Dispose()
        {
            TcpClient?.Close();
            TcpClient?.Dispose();
        }
    }
}