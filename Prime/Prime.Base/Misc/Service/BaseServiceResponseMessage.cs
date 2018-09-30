namespace Prime.Core
{
    public abstract class BaseServiceResponseMessage : BaseTransportResponseMessage, IHasServiceStatus
    {
        protected BaseServiceResponseMessage(BaseTransportRequestMessage request, ServiceStatus status) : base(request)
        {
            ServiceStatus = status;
        }

        public ServiceStatus ServiceStatus { get; }
    }
}