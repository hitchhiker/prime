using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Core;

namespace Prime.Radiant.Catalogue
{
    public static class Helper
    {
        public static Type GetCatalogueType(PrimeInstance instance, string catalogueName)
        {
            var list = instance.ExtensionManager.Types
                .Where(x => x.Name.Contains("package", StringComparison.OrdinalIgnoreCase)).ToList();

            var inst = instance.ExtensionManager.Types.ImplementInstancesWith<ICatalogue>().FirstOrDefault(x=>string.Equals(catalogueName, x.CatalogueTypeName, StringComparison.OrdinalIgnoreCase));
            return inst?.GetType();
        }
    }
}
