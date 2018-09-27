using Prime.Base.DStore;

namespace Prime.Core
{
    public class GetContentResponse : BaseTransportResponseMessage
    {
        private GetContentResponse() { }

        public GetContentResponse(GetContentRequest request, string protocol) : base(request)
        {
        }

        public bool Success { get; set; }
    }
}