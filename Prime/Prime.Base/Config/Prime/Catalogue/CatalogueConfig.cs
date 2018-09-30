using System.Xml.Serialization;

namespace Prime.Core
{
    public class CatalogueConfig
    {
        [XmlElement("subscribed")]
        public CatalogueSubscribedConfig Subscribed { get; set; } = new CatalogueSubscribedConfig();
    }
}