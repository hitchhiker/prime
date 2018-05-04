namespace Prime.Core
{
    public interface IExtensionExecute : IExtension
    {
        void Main(AppContext context);
    }
}