namespace Prime.Radiant
{
    public interface ICatalogueBuilder
    {
        ICatalogue Build();

        ICatalogue CompileCatalogue();

        string TypeName { get; }
    }
}