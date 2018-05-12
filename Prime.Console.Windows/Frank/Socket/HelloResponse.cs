using Prime.Core;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class HelloResponse : BaseTransportResponseMessage
    {
        public HelloResponse() { }

        public HelloResponse(HelloRequest request) : base(request) { }

        public string Response => "Hello World of Sockets!";
    }
}