using Prime.Base.DStore;

namespace Prime.Core
{
    public class GetNsResolveResponse : BaseTransportResponseMessage
    {
        private GetNsResolveResponse() { }

        public GetNsResolveResponse(GetNsResolveRequest request, string protocol) : base(request)
        {
            Protocol = protocol;
        }

        public string Protocol { get; set; }

        public ContentUri ContentUri { get; set; }

        public bool Success { get; set; }
    }
}