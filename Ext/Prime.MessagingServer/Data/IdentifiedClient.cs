using System;
using System.Net.Sockets;
using Prime.Base;

namespace Prime.MessagingServer.Data
{
    public class IdentifiedClient : IDisposable
    {
        public readonly Socket ClientSocket;
        public readonly ObjectId Id;

        public IdentifiedClient(Socket clientSocket)
        {
            if (clientSocket == null)
                return;

            Id = ObjectId.NewObjectId();
            ClientSocket = clientSocket;
        }

        public void Dispose()
        {
            ClientSocket?.Disconnect(true);
            ClientSocket?.Close();
            ClientSocket?.Dispose();
        }
    }
}