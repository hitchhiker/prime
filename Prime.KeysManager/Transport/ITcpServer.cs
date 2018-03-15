using System;
using System.Net;

namespace Prime.KeysManager.Transport
{
    public interface ITcpServer
    {
        void StartServer(IPAddress address, short port);
        void ShutdownServer();

        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>();

        void Send<T>(T data);

        event EventHandler<Exception> ExceptionOccurred;
    }
}