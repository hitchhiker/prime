namespace Prime.Core
{
    public interface IExtensionExecute : IExtension
    {
        void Main(PrimeContext context);
    }
}