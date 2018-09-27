using Prime.Base;

namespace Prime.Core
{
    public class PinContentRequest : BaseTransportRequestMessage
    {
        private PinContentRequest() { }

        public PinContentRequest(string localHash, string protocol = "ipfs")
        {
            SessionId = ObjectId.NewObjectId();
            LocalHash = localHash;
            Protocol = protocol;
        }

        public string LocalHash { get; set; }

        public string Protocol { get; set; }
    }
}