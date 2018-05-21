namespace Prime.Core
{
    public abstract class BooleanResponseMessage : BaseTransportResponseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        protected BooleanResponseMessage()
        {
            Success = true;
        }

        protected BooleanResponseMessage(BaseTransportRequestMessage request, bool success, string message = null) : base(request)
        {
            Success = success;
            SessionId = request.SessionId;
            Message = message;
        }
    }
}