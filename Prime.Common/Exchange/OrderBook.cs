using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class OrderBook : IReadOnlyList<OrderBookRecord>, ISerialiseAsObject
    {
        public readonly Network Network;
        private readonly List<OrderBookRecord> _records = new List<OrderBookRecord>();
        private readonly List<OrderBookRecord> _asks = new List<OrderBookRecord>();
        private readonly List<OrderBookRecord> _bids = new List<OrderBookRecord>();

        public IReadOnlyList<OrderBookRecord> Asks => _asks;
        public IReadOnlyList<OrderBookRecord> Bids => _bids;

        public OrderBook(Network network)
        {
            Network = network;
        }

        public void Add(OrderBookRecord record)
        {
            if (record.Type == Exchange.OrderBookType.Ask)
                _asks.Add(record);
            else
                _bids.Add(record);
            _records.Add(record);
        }

        public void AddRange(IEnumerable<OrderBookRecord> records)
        {
            foreach (var r in records)
                Add(r);
        }

        private Money? _spread;
        public Money Spread => _spread ?? (Money)(_spread = Money.Zero);

        private OrderBookRecord _ask;
        public OrderBookRecord HighestAsk => _ask ?? (_ask = _asks.OrderByDescending(x => x.Price).FirstOrDefault());

        private OrderBookRecord _bid;
        public OrderBookRecord LowestBid => _bid ?? (_bid = _bids.OrderBy(x => x.Price).FirstOrDefault());


        public IEnumerator<OrderBookRecord> GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _records).GetEnumerator();

        public int Count => _records.Count;

        public OrderBookRecord this[int index] => _records[index];
    }
}
