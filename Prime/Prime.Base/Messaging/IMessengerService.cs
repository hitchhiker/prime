using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core
{
    public interface IMessengerService
    {
        IMessenger M { get; }
        void Start(PrimeContext context);
        void Stop();
    }
}