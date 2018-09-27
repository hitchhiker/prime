using Prime.Base.DStore;

namespace Prime.Radiant
{
    public interface ICatalogueBuilder
    {
        ContentUri Build();

        ICatalogue CompileCatalogue();

        string TypeName { get; }
    }
}