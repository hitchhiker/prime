using System.Collections.Generic;
using Prime.Base.DStore;
using Prime.Core;

namespace Prime.Radiant
{
    public interface ICatalogue
    {
        string PublicKey { get; set; }

        string CatalogueTypeName { get; }

        string Name { get; set; }

        CatalogueBuildInformation BuildInformation { get; set; }

        List<ContentUri> AllContentUri();

        bool DoInstall(PrimeInstance instance);
    }
}