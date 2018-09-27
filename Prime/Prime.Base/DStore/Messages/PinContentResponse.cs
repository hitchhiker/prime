using Prime.Base.DStore;

namespace Prime.Core
{
    public class PinContentResponse : BaseTransportResponseMessage
    {
        private PinContentResponse() { }

        public PinContentResponse(PinContentRequest request, string protocol) : base(request)
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