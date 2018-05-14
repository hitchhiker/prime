namespace Prime.Core
{
    public interface IMessageServerExtension : IExtension
    {
        void Start(MessagingServer server);

        void Stop();

        void Send(BaseTransportMessage message);
    }
}