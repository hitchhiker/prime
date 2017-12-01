using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public abstract class ModelBase : IModelBase
    {
        protected ObjectId IdBacking;

        /// <summary>
        /// This is the internal unique ID of this document, stored as a 12 byte ObjectId
        /// If one doesn't exist, it will be generated and stored for the lifetime of this object.
        /// </summary>
        [BsonId]
        public ObjectId Id
        {
            get => IdBacking.IsNullOrEmpty() ? (IdBacking = GenerateId()) : IdBacking;
            set => IdBacking = value;
        }

        public virtual ObjectId GenerateId()
        {
            return ObjectId.NewObjectId();
        }
    }
}