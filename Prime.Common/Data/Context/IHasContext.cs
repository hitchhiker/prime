namespace Prime.Common
{
    public interface IHasContext
    {
        IDataContext Context { get; set; }
    }
}