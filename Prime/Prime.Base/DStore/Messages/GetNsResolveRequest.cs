using Prime.Base;

namespace Prime.Core
{
    public class GetNsResolveRequest : BaseTransportRequestMessage
    {
        private GetNsResolveRequest() { }

        public GetNsResolveRequest(string remotePath, string protocol = "ipfs")
        {
            SessionId = ObjectId.NewObjectId();
            RemotePath = remotePath;
            Protocol = protocol;
        }

        public string RemotePath { get; set; }

        public string Protocol { get; set; }
    }
}