using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.SocketServer.Transport
{
    public enum TcpSocketEventType
    {
        ClientConnected,
        ErrorOccurred,
        ClientDisconnected,
        DataReceived
    }
}
