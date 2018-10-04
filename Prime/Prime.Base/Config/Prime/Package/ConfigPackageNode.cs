using System.Xml.Serialization;
using Prime.Base;

namespace Prime.Core
{
    public class ConfigPackageNode
    {
        [XmlElement("redirects")]
        public RedirectConfig RedirectConfig { get; set; } = new RedirectConfig();

        [XmlElement("installed")]
        public InstallConfig InstallConfig { get; set; } = new InstallConfig();
    }
}