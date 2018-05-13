using Prime.Core;

namespace Prime.WebSocketServer.Messages
{
    public class HelloWsResponse : BaseTransportResponseMessage
    {
        public string Response => "WS hello world!";

        public HelloWsResponse(HelloWsRequest request) : base(request)
        {
            
        }
    }
}
