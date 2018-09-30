namespace Prime.Core
{
    public interface IExtensionGracefullShutdown : IExtension
    {
        bool HasShutdown { get; }
    }
}