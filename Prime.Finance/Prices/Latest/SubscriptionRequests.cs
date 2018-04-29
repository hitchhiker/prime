using Prime.Core;

namespace Prime.Finance.Prices.Latest
{
    internal sealed class SubscriptionRequests
    {
        internal readonly UniqueList<Request> Requests = new UniqueList<Request>();
    }
}