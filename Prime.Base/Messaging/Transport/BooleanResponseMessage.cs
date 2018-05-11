namespace Prime.Core
{
    public abstract class BooleanResponseMessage : BaseTransportResponseMessage
    {
        public bool Success { get; set; }

        protected BooleanResponseMessage() { }

        protected BooleanResponseMessage(BaseTransportRequestMessage request, bool success) : base(request)
        {
            Success = success;
            ClientId = request.ClientId;
        }
    }
}