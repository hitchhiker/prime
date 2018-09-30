using Prime.Core;

namespace Prime.Settings
{
    public class TimeKindUiUpdatedMessage : BaseTransportRequestMessage
    {
        public bool IsUtc { get; set; }
    }
}