using Prime.Core;

namespace Prime.Manager.Messages
{
    public class ProviderSaveKeysResponseMessage : BooleanResponseMessage
    {
        public ProviderSaveKeysResponseMessage(ProviderSaveKeysRequestMessage request, bool success, string message = null) : base(request, success, message) { }
    }
}