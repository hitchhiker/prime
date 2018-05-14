using Prime.Core;

namespace Prime.MessagingServer
{
    public interface IMessageServerExtension : IExtension
    {
        void Start(Server server);

        void Stop();

        void Send(BaseTransportMessage message);
    }
}