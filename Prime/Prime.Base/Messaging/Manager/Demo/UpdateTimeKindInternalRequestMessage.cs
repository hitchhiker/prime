using Prime.Core;

namespace Prime.Settings
{
    public class UpdateTimeKindInternalRequestMessage : BaseTransportRequestMessage
    {
        public bool IsUtc { get; set; }
    }
}