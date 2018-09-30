using System.Xml.Serialization;
using Prime.Base;

namespace Prime.Core
{
    public class InstallEntry : IUniqueIdentifier<ObjectId>
    {
        [XmlAttribute("id")]
        public string IdString { get; set; }

        private ObjectId _id;
        [XmlIgnore]
        public ObjectId Id => _id ?? (_id = new ObjectId(IdString));
    }
}