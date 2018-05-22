using Prime.Core;

namespace Prime.Manager.Messages
{
    public class TestPrivateApiResponseMessage : BooleanResponseMessage
    {
        public TestPrivateApiResponseMessage(TestPrivateApiRequestMessage request, bool success, string message = null) : base(request, success, message)
        {
        }
    }
}