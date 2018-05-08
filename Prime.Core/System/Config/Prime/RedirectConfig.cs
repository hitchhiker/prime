using System.Collections.Generic;
using System.Xml.Serialization;

namespace Prime.Core
{
    public class RedirectConfig
    {
        [XmlElement("redirect")]
        public List<RedirectEntry> Redirects { get; set; } = new List<RedirectEntry>();
    }
}