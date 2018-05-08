using System;
using System.Net;
using System.Net.Sockets;

namespace Prime.KeysManager.Transport
{
    public interface ITcpServer
    {
        void StartServer(IPAddress address, short port);
        void ShutdownServer();

        void Subscribe<T>(Action<T, TcpClient> handler);
        void Unsubscribe<T>();

        void Send<T>(TcpClient client, T data);

        event EventHandler<Exception> ExceptionOccurred;
    }
}