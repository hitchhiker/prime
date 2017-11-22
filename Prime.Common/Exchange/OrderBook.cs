using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Common.Exchange;
using Prime.Utility;

namespace Prime.Common
{        /// <summary>
    /// TODO: Just a note to Alex (delete when read)
    /// Added network and pair to constructor
    /// Added IREADONLYLIST as base, so we entries cant be accidentaly modified
    /// Add 'Add' method to include Money conversion, less likely to have mistakes in providers.
    /// Check for correct quote asset when adding also.
    /// TODO: Update your other providers to work like 'CoinBase/Korbit' add/ask etc
    /// </summary>
    public class OrderBook : IReadOnlyList<OrderBookRecord>, ISerialiseAsObject
    {
        public readonly AssetPair Pair;
        public readonly Network Network;
        private readonly List<OrderBookRecord> _records = new List<OrderBookRecord>();
        private readonly List<OrderBookRecord> _asks = new List<OrderBookRecord>();
        private readonly List<OrderBookRecord> _bids = new List<OrderBookRecord>();

        public IReadOnlyList<OrderBookRecord> Asks => _asks;
        public IReadOnlyList<OrderBookRecord> Bids => _bids;

        public OrderBook(Network network, AssetPair pair)
        {
            Pair = pair;
            Network = network;
        }

        public void AddAsk(Money price, decimal volume)
        {
            Add(new OrderBookRecord(OrderType.Ask, new Money(price, Pair.Asset2), volume));
        }

        public void AddBid(Money price, decimal volume)
        {
            Add(new OrderBookRecord(OrderType.Bid, new Money(price, Pair.Asset2), volume));
        }

        public void Add(OrderType type, Money price, decimal volume)
        {
            Add(new OrderBookRecord(type, new Money(price, Pair.Asset2), volume));
        }

        public void Add(OrderBookRecord record)
        {
            if (record.Price == 0 || record.Volume == 0)
                return;

            if (record.Price.Asset.Id!= Pair.Asset2.Id)
                throw new System.Exception($"You cant add this {nameof(OrderBookRecord)} as it has the wrong asset: {record.Price.Asset} -> should be: {Pair.Asset2}");

            if (record.Type == OrderType.Ask)
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
        public Money Spread => _spread ?? (Money)(_spread = LowestAsk.Price - HighestBid.Price);

        private decimal? _spreadPercentage;
        public decimal SpreadPercentage => _spreadPercentage ?? (decimal)(_spreadPercentage = HighestBid.Price.PercentageProfit(LowestAsk.Price));

        private OrderBookRecord _lowestAsk;
        public OrderBookRecord LowestAsk => _lowestAsk ?? (_lowestAsk = _asks.OrderBy(x => x.Price).FirstOrDefault());

        private OrderBookRecord _highestBid;
        public OrderBookRecord HighestBid => _highestBid ?? (_highestBid = _bids.OrderByDescending(x => x.Price).FirstOrDefault());

        public Money PriceAtPercentage(OrderType type, decimal percentage)
        {
            var top = type == OrderType.Ask ? LowestAsk : HighestBid;
            var price = type == OrderType.Ask ? top.Price.PercentageAdd(percentage) : top.Price.PercentageAdd(-percentage);
            return price;
        }

        public Money VolumeAt(OrderType type, Money price)
        {
            return type == OrderType.Ask
                ? _asks.Where(x => x.Price <= price).Select(x => x.Volume).Sum(price.Asset)
                : _bids.Where(x => x.Price >= price).Select(x => x.Volume).Sum(price.Asset);
        }

        public Money VolumeAtPercentage(OrderType type, decimal percentage)
        {
            return VolumeAt(type, PriceAtPercentage(type, percentage));
        }

        public IEnumerator<OrderBookRecord> GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _records).GetEnumerator();

        public int Count => _records.Count;

        public OrderBookRecord this[int index] => _records[index];
    }
}
