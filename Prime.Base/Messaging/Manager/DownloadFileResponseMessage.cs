using Prime.Core;

namespace Prime.Manager.Messages
{
    public class DownloadFileResponseMessage : BaseTransportResponseMessage
    {
        public DownloadFileResponseMessage(DownloadFileRequestMessage requestMsg) : base(requestMsg) { }

        public string FileBase64 { get; set; }
    }
}
