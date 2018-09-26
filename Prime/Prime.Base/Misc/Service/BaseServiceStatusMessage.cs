namespace Prime.Core
{
    public abstract class BaseServiceStatusMessage : BaseTransportMessage, IHasServiceStatus
    {
        protected BaseServiceStatusMessage(ServiceStatus status)
        {
            ServiceStatus = status;
        }

        public ServiceStatus ServiceStatus { get; }
    }
}