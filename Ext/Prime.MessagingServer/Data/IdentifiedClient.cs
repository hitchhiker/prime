using System;
using System.Dynamic;
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
            ClientSocket = clientSocket ?? throw new ArgumentNullException(nameof(clientSocket));
            Id = ObjectId.NewObjectId();

            CreatedUtc = DateTime.UtcNow;
        }

        public void UpdateLastRead() => LastReadUtc = DateTime.UtcNow;
        public void UpdateLastWrite() => LastWriteUtc = DateTime.UtcNow;

        public DateTime? LastReadUtc { get; private set; }
        public DateTime? LastWriteUtc { get; private set; }

        public DateTime? LastActivityUtc
        {
            get
            {
                if (LastWriteUtc.HasValue && LastReadUtc.HasValue)
                    return LastWriteUtc.Value > LastReadUtc.Value ? LastWriteUtc : LastReadUtc;

                if (LastReadUtc.HasValue ^ LastWriteUtc.HasValue)
                    return LastReadUtc ?? LastWriteUtc;

                return null;
            }
        }

        public DateTime CreatedUtc { get; set; }

        public void Dispose()
        {
            ClientSocket?.Disconnect(true);
            ClientSocket?.Close();
            ClientSocket?.Dispose();
        }
    }
}