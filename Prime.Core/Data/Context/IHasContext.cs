namespace Prime.Core
{
    public interface IHasContext
    {
        IDataContext Context { get; set; }
    }
}