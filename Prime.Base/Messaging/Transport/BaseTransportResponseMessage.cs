namespace Prime.Core
{
    public abstract class BaseTransportResponseMessage : BaseTransportMessage
    {
        protected BaseTransportResponseMessage() { }

        protected BaseTransportResponseMessage(BaseTransportRequestMessage request)
        {
            SessionId = request.SessionId;
        }
    }
}