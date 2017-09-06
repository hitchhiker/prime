namespace Prime.Utility
{
    public interface IUniqueIdentifier<out T>
    {
        T Id { get; }
    }
}