using Prime.Base;

namespace Prime.Core
{
    public interface IExtensionInitPrimeInstance : IExtension
    {
        void Init(PrimeInstance instance);
    }
}