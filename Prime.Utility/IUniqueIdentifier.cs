namespace Prime.Common
{
    public interface IUniqueIdentifier<out T>
    {
        T Id { get; }
    }
}