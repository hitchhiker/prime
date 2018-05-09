namespace Prime.Core
{
    public interface IUniqueIdentifier<out T>
    {
        T Id { get; }
    }
}