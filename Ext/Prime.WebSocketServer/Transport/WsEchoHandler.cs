using WebSocketSharp;

namespace Prime.WebSocketServer.Transport
{
    public class WsEchoHandler : WsHandlerBase
    {
        public const string ServicePath = "/echo";
        
        protected override void OnMessage(MessageEventArgs e)
        {
            Send(e.Data);
        }
    }
}