using System.Xml.Serialization;

namespace Prime.Core
{
    public class PackageConfig
    {
        [XmlElement("redirects")]
        public RedirectConfig RedirectConfig { get; set; } = new RedirectConfig();

        [XmlElement("installed")]
        public InstallConfig InstallConfig { get; set; } = new InstallConfig();
    }
}