using System;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public interface INetworkProvider : IUniqueIdentifier<ObjectId>
    {
        Network Network { get; }

        bool Disabled { get; }

        int Priority { get; }

        string AggregatorName { get; }

        string Title { get; }
    }
}