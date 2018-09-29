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
            foreach (var ext in instance.ExtensionManager.Instances)
            {
                var list = ext.Extension.GetType().Assembly.GetTypes().Where(x => x.Name.Contains("package", StringComparison.OrdinalIgnoreCase)).ToList();
                var inst = list.ImplementInstancesWith<ICatalogue>().FirstOrDefault(x => string.Equals(catalogueName, x.CatalogueTypeName, StringComparison.OrdinalIgnoreCase));

                if (inst!=null)
                    return inst.GetType();
            }
            return null;
        }
    }
}
