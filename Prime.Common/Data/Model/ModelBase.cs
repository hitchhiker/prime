using LiteDB;

namespace Prime.Common
{
    public abstract class ModelBase : IModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}