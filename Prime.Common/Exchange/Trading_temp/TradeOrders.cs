using System.Collections;
using System.Collections.Generic;

namespace Prime.Common
{
    public class TradeOrders : IReadOnlyList<TradeOrder>
    {
        public readonly Network Network;

        public TradeOrders(Network network)
        {
            Network = network;
        }

        private readonly List<TradeOrder> _orders = new List<TradeOrder>();

        public IReadOnlyList<TradeOrder> Orders => _orders;


        public void Add(TradeOrder order)
        {
            _orders.Add(order);
        }

        public IEnumerator<TradeOrder> GetEnumerator() => _orders.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _orders).GetEnumerator();

        public int Count => _orders.Count;

        public TradeOrder this[int index] => _orders[index];
    }
}