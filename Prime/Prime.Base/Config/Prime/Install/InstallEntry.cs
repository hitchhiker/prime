using System;
using System.Xml.Serialization;
using Prime.Base;

namespace Prime.Core
{
    public class InstallEntry : IUniqueIdentifier<ObjectId>
    {
        [XmlAttribute("id")]
        public string IdString { get; set; }

        [XmlAttribute("version")]
        public string VersionString { get; set; }

        private ObjectId _id;
        [XmlIgnore]
        public ObjectId Id => _id ?? (_id = new ObjectId(IdString));

        private Version _version;
        [XmlIgnore]
        public Version Version => _version ?? (_version = string.IsNullOrWhiteSpace(VersionString) ? null : new Version(VersionString));
    }
}