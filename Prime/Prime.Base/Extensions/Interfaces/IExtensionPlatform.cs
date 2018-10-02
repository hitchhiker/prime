using Prime.Core;

namespace Prime.Base
{
    public interface IExtensionPlatform : IExtension
    {
        Platform Platform { get; }
    }
}