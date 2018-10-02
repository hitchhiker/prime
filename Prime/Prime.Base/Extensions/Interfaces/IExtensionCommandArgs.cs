using Prime.Core;

namespace Prime.Base
{
    public interface IExtensionCommandArgs : IExtension
    {
        void InitialiseCommandArgs(PrimeContext context, CommandArgs args);
    }
}