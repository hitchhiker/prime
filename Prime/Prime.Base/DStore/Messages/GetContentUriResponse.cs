using Prime.Base.DStore;

namespace Prime.Core
{
    public class GetContentUriResponse : BaseTransportResponseMessage
    {
        private GetContentUriResponse() { }

        public GetContentUriResponse(GetContentUriRequest request, string protocol) : base(request)
        {
            LocalPath = request.LocalPath;
            Protocol = protocol;
        }

        public string LocalPath { get; set; }

        public string Protocol { get; set; }

        public ContentUri ContentUri { get; set; }

        public bool Success { get; set; }
    }
}