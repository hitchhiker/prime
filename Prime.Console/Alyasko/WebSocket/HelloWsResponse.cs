using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Console.Alyasko.WebSocket
{
    public class HelloWsResponse : BaseTransportResponseMessage
    {
        public string Response => "WS hello world!";

        public HelloWsResponse(HelloWsRequest request) : base(request)
        {
            
        }
    }
}
