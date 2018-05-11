using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class TestPrivateApiResponseMessage : BooleanResponseMessage
    {
        public TestPrivateApiResponseMessage(TestPrivateApiRequestMessage request, bool success) : base(request, success)
        {
        }
    }
}