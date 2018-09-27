using System.Collections.Generic;
using Prime.Base.DStore;

namespace Prime.Radiant
{
    public interface ICatalogue
    {
        string CatalogueTypeName { get; }

        string Name { get; set; }

        CatalogueBuildInformation BuildInformation { get; set; }

        List<ContentUri> AllContentUri();
    }
}