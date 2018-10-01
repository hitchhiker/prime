using System.Xml.Serialization;

namespace Prime.Core
{
    public class CatalogueSubscribedUriEntry
    {
        [XmlAttribute("uri")]
        public string UriString { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}