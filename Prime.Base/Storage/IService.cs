using Prime.Core;

namespace Prime.Base.Storage
{
    public interface IService
    {
        void Start();
        void Stop();
        ServiceStatus GetStatus();
    }
}