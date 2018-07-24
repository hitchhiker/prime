using System;
using System.Collections.Generic;
using System.Text;
using Prime.Base;

namespace Prime.SocketServer.Transport
{
    public class TcpSocketEventArgs : EventArgs
    {
        public TcpSocketEventArgs(TcpSocketEventType eventType)
        {
            EventType = eventType;
        }

        public byte[] Data { get; set; }
        public ObjectId ClientId { get; set; }
        public Exception Exception { get; set; }

        public TcpSocketEventType EventType { get; set; }
    }
}
