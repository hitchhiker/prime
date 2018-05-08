using System.IO;
using System.Xml.Serialization;

namespace Prime.Core
{
    public class RedirectEntry
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlIgnore]
        public DirectoryInfo DirectoryInfo => new DirectoryInfo(Path);
    }
}