using LiteDB;

namespace Prime.Core
{
    public class Remote
    {
        [Bson]
        public string Id { get; set; }

        [Bson]
        public ObjectId ServiceId { get; set; }
    }
}