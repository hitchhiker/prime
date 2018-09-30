using Prime.Base;

namespace Prime.Core
{
    public class PublishNsRequest : BaseTransportRequestMessage
    {
        private PublishNsRequest() { }

        public PublishNsRequest(string localHash, string key = "self", string protocol = "ipfs")
        {
            SessionId = ObjectId.NewObjectId();
            Key = key;
            LocalHash = localHash;
            Protocol = protocol;
        }

        public string Key { get; set; }

        public string LocalHash { get; set; }

        public string Protocol { get; set; }
    }
}