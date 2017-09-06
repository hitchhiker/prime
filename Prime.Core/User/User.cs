using System;
using LiteDB;

namespace Prime.Core
{
    public class User : IModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

    }
}