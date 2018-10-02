using System;
using Prime.Base;
using Prime.Core;

namespace Prime.Base
{
    public interface IExtension : IUniqueIdentifier<ObjectId>
    {
        string Title { get; }

        Version Version { get; }
    }
}