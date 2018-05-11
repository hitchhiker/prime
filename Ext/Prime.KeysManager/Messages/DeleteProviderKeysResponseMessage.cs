using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class DeleteProviderKeysResponseMessage : BooleanResponseMessage
    {
        public DeleteProviderKeysResponseMessage(DeleteProviderKeysRequestMessage request, bool success) : base(request,success) { }
    }
}