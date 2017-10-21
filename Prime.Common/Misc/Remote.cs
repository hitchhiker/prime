using LiteDB;

namespace Prime.Common
{
    public class Remote
    {
        [Bson]
        public string Id { get; set; }

        [Bson]
        public ObjectId ServiceId { get; set; }
    }
}