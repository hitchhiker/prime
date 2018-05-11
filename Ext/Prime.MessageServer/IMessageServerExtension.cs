namespace Prime.Core
{
    public interface IMessageServerExtension : IExtension
    {
        void Start(MessageServer server);

        void Stop();

        void Send(BaseTransportMessage message);
    }
}