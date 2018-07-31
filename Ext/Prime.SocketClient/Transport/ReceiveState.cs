using System;

namespace Prime.SocketClient.Transport
{
    public class ReceiveState
    {
        private const int DefaultBufferSize = 1024;
        
        public ReceiveState()
        {
            
        }
        
        public UInt32? ExpectedMessageSize { get; set; }
        public byte[] Buffer { get; } = new byte[DefaultBufferSize];
    }
}