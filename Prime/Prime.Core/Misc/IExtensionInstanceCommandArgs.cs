using Prime.Core;

namespace Prime.Base
{
    public interface IExtensionInstanceCommandArgs : IExtension
    {
        void InitialiseCommandArgs(PrimeInstance instance, CommandArgs argsParser);
    }
}