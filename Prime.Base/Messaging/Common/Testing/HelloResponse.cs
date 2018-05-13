using Newtonsoft.Json;
using Prime.Core;

namespace Prime.Core.Testing
{
    public class HelloResponse : BaseTransportResponseMessage
    {
        public HelloResponse() { }

        public HelloResponse(HelloRequest request, string response) : base(request)
        {
            Response = response;
        }

        [JsonProperty("response")]
        public string Response { get; set; }
    }
}