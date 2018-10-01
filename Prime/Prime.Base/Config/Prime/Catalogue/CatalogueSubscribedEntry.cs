using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Prime.Base.DStore;

namespace Prime.Core
{
    public class CatalogueSubscribedEntry
    {
        [XmlAttribute("pub")]
        public string PublicKey { get; set; }

        [XmlElement("entry")]
        public List<CatalogueSubscribedUriEntry> Entries { get; set; } = new List<CatalogueSubscribedUriEntry>();

        public ContentUri GetRandomNotSelf()
        {
            return Entries.Where(x => x.Type != "self").Select(x=> ContentUri.Parse(x.UriString)).FirstOrDefault();
        }
        public ContentUri GetSelf()
        {
            return Entries.Where(x => x.Type == "self").Select(x => ContentUri.Parse(x.UriString)).FirstOrDefault();
        }
    }
}