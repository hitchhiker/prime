using Prime.Base;
using Prime.Base.DStore;

namespace Prime.Core
{
    public class PinContentRequest : BaseTransportRequestMessage
    {
        private PinContentRequest() { }

        public PinContentRequest(ContentUri uri) : this(uri.Path, uri.Protocol) { }

        public PinContentRequest(string localPath, string protocol = "ipfs")
        {
            SessionId = ObjectId.NewObjectId();
            LocalPath = localPath;
            Protocol = protocol;
        }

        public string LocalPath { get; set; }

        public string Protocol { get; set; }
    }
}