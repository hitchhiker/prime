using Prime.Base;
using Prime.Core;

namespace Prime.Base
{
    public interface IExtensionExecute : IExtension
    {
        void Main(PrimeContext context);
    }
}