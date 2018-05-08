using System.Xml.Serialization;
using LiteDB;

namespace Prime.Core
{
    public class InstallEntry : IUniqueIdentifier<ObjectId>
    {
        [XmlAttribute("id")]
        public string IdString { get; set; }

        [XmlIgnore]
        private ObjectId _id;

        [XmlIgnore]
        public ObjectId Id => _id ?? (_id = new ObjectId(IdString));
    }
}