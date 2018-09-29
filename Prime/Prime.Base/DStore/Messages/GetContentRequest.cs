using Prime.Base;

namespace Prime.Core
{
    public class GetContentRequest : BaseTransportRequestMessage
    {
        private GetContentRequest() { }

        public GetContentRequest(string localPath, string remotePath, string protocol = "ipfs")
        {
            SessionId = ObjectId.NewObjectId();
            LocalPath = localPath;
            RemotePath = remotePath;
            Protocol = protocol;
        }

        public string RemotePath { get; set; }

        public bool IsDirectory { get; set; }

        public string LocalPath { get; set; }

        public string Protocol { get; set; }
    }
}