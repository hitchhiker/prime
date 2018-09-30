using System.Collections.Generic;
using System.Xml.Serialization;

namespace Prime.Core
{
    public class CatalogueSubscribedConfig
    {
        [XmlElement("catalogue")]
        public List<CatalogueSubscribedEntry> Entries { get; set; } = new List<CatalogueSubscribedEntry>();
    }
}