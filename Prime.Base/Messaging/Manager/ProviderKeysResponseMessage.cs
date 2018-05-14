using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderKeysResponseMessage : BooleanResponseMessage
    {
        public ProviderKeysResponseMessage(ProviderKeysRequestMessage request, bool success) : base(request, success) { }
    }
}