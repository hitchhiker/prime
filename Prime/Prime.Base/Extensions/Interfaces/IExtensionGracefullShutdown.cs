namespace Prime.Base
{
    public interface IExtensionGracefullShutdown : IExtension
    {
        bool HasShutdown { get; }
    }
}