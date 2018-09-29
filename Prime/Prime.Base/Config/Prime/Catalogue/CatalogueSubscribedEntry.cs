using System.Xml.Serialization;

namespace Prime.Core
{
    public class CatalogueSubscribedEntry
    {
        [XmlAttribute("uri")]
        public string UriString { get; set; }

        [XmlAttribute("pub")]
        public string PublicKey { get; set; }
    }
}