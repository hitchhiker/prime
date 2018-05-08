using System;
using LiteDB;

namespace Prime.Core
{
    public interface IExtension : IUniqueIdentifier<ObjectId>
    {
        string Title { get; }

        Version Version { get; }
    }
}