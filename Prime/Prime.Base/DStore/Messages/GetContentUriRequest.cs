using Prime.Base;

namespace Prime.Core
{
    public class GetContentUriRequest : BaseTransportRequestMessage
    {
        private GetContentUriRequest() { }

        public GetContentUriRequest(string localPath, string protocol = "ipfs")
        {
            SessionId = ObjectId.NewObjectId();
            LocalPath = localPath;
            Protocol = protocol;
        }

        public string LocalPath { get; set; }

        public string Protocol { get; set; }
    }
}