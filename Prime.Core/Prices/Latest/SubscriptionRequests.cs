﻿using Prime.Common;

namespace Prime.Core.Prices.Latest
{
    internal sealed class SubscriptionRequests
    {
        internal readonly UniqueList<Request> Requests = new UniqueList<Request>();
    }
}