using LiteDB;

namespace Prime.Core
{
    public abstract class ModelBase : IModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}