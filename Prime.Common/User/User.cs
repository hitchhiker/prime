using System;
using LiteDB;

namespace Prime.Common
{
    public class User : IModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

    }
}