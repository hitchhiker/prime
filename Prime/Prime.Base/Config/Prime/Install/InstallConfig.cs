using System.Collections.Generic;
using System.Xml.Serialization;

namespace Prime.Core
{
    public class InstallConfig
    {
        [XmlElement("install")]
        public List<InstallEntry> Installs { get; set; } = new List<InstallEntry>();

        [XmlAttribute("entry")]
        public string EntryPointCore { get; set; }
    }
}