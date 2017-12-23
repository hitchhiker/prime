using System.Collections.Generic;

namespace Prime.Common
{
    public class PlacedOrderLimitResponse
    {
        public PlacedOrderLimitResponse(string remoteOrderGroupId)
        {
            RemoteOrderGroupId = remoteOrderGroupId;
        }

        public PlacedOrderLimitResponse(string remoteOrderGroupId, IEnumerable<string> remoteOrderIds) : this(remoteOrderGroupId)
        {
            RemoteOrderIds.AddRange(remoteOrderIds);
        }

        /// <summary>
        /// When an order is placed, it may result in one, or a number of orders (or trades).
        /// This is the REMOTE ID that represents that group of orders.
        /// </summary>
        public string RemoteOrderGroupId { get; }

        public List<string> RemoteOrderIds { get; } = new List<string>();
    }
}