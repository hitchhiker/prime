using Prime.Core;

namespace Prime.Settings
{
    public class UpdateWebUiTimeKindRequestMessage : BaseTransportRequestMessage
    {
        public bool IsUtc { get; set; }
    }
}