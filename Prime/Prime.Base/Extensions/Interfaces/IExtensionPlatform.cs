namespace Prime.Core
{
    public interface IExtensionPlatform : IExtension
    {
        Platform Platform { get; }
    }
}