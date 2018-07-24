using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prime.Base;

namespace Prime.SocketServer.Transport
{
    /// <summary>
    /// Inspired by https://www.codeproject.com/Articles/13232/A-very-basic-TCP-server-written-in-C
    /// </summary>
    public abstract class SocketServiceProvider : ICloneable
    {
        public virtual object Clone()
        {
            throw new Exception("Derived class must override Clone() method.");
        }

        public abstract void OnClientConnected(ObjectId clientId);
        public abstract void OnClientDisconnected(ObjectId clientId);
        public abstract void OnErrorOccurred(Exception e, ObjectId clientId);
        public abstract void OnDataReceived(ObjectId clientId, byte[] clonedData);
        public abstract void OnServerStarted(IPEndPoint ipEndPoint);
        public abstract void OnServerStopped();

        public abstract void Send<T>(ObjectId clientId, T data);

        public TcpSocketServer TcpSocketServer { get; set; }
    }
}
