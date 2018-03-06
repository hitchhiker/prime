using System;
using System.Net;
using Prime.KeysManager.Events;

namespace Prime.KeysManager.Transport
{
    public interface ITcpServer
    {
        void CreateServer(IPAddress address, short port);
        void ShutdownServer();

        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>();

        event EventHandler<Exception> ExceptionOccurred;
    }
}