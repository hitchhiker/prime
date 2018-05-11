using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class ProviderKeysResponseMessage : BooleanResponseMessage
    {
        public ProviderKeysResponseMessage(ProviderKeysRequestMessage request, bool success) : base(request, success) { }
    }
}