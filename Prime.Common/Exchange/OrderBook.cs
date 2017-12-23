using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Common.Exchange;
using Prime.Utility;
using System;

namespace Prime.Common
{
    /// <summary>
    /// TODO: Just a note to Alex (delete when read)
    /// Added network and pair to constructor
    /// Added IREADONLYLIST as base, so we entries cant be accidentaly modified
    /// Add 'Add' method to include Money conversion, less likely to have mistakes in providers.
    /// Check for correct quote asset when adding also.
    /// TODO: Update your other providers to work like 'CoinBase/Korbit' add/ask etc
    /// </summary>
    public class OrderBook : ISerialiseAsObject
    {
        public readonly AssetPair Pair;
        public readonly AssetPair PairNormalised;
        public readonly Network Network;
        private List<OrderBookRecord> _asks = new List<OrderBookRecord>();
        private List<OrderBookRecord> _bids = new List<OrderBookRecord>();
        public bool IsSorted { get; private set; } = true;

        public IReadOnlyList<OrderBookRecord> Asks => _asks;
        public IReadOnlyList<OrderBookRecord> Bids => _bids;

        public readonly DateTime UtcCreated;

        public OrderBook(Network network, AssetPair pair, IEnumerable<OrderBookRecord> asks, IEnumerable<OrderBookRecord> bids) : this(network, pair)
        {
            asks.ForEach(Add);
            bids.ForEach(Add);
        }

        public OrderBook(Network network, AssetPair pair)
        {
            Pair = pair;
            PairNormalised = pair.Normalised;
            Network = network;
            UtcCreated = DateTime.UtcNow;
        }

        public void AddAsk(decimal price, decimal volume, bool volumeIsNotQuote = false)
        {
            Add(OrderType.Ask, price, volume, volumeIsNotQuote);
        }

        public void AddBid(decimal price, decimal volume, bool volumeIsNotQuote = false)
        {
            Add(OrderType.Bid, price, volume, volumeIsNotQuote);
        }

        public void Add(OrderType type, decimal price, decimal volume, bool volumeIsNotQuote = false)
        {
            Add(OrderBookRecord.CreateInternal(type, new Money(price, Pair.Asset2), GetVolume(price, volume, volumeIsNotQuote)));
        }

        private Money GetVolume(decimal price, decimal volume, bool volumeIsNotQuote)
        {
            return volumeIsNotQuote ? new Money(volume * price, Pair.Asset2) : new Money(volume, Pair.Asset2);
        }

        public void Add(OrderBookRecord record)
        {
            if (record.Price == 0 || record.Volume == 0)
                return;

            if (record.Price.Asset.Id != Pair.Asset2.Id)
                throw new System.Exception($"You cant add this {nameof(OrderBookRecord)} as it has the wrong asset: {record.Price.Asset} -> should be: {Pair.Asset2}");

            if (record.Volume.Asset.Id != Pair.Asset2.Id)
                throw new System.Exception($"You cant add this {nameof(OrderBookRecord)} as it has the wrong volume asset: {record.Volume.Asset} -> should be: {Pair.Asset2}");

            IsSorted = IsSorted && IsInOrder(record);

            if (record.Type == OrderType.Ask)
                _asks.Add(record);
            else
                _bids.Add(record);
        }

        private bool IsInOrder(OrderBookRecord newRecord)
        {
            if (newRecord.Type == OrderType.Ask)
            {
                var la = _asks.LastOrDefault();
                return la == null || newRecord.Price > la.Price;
            }

            var lb = _bids.LastOrDefault();
            return lb == null || newRecord.Price < lb.Price;
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

        public Money Price => new Money((LowestAsk.Price + HighestBid.Price) / 2, LowestAsk.Price.Asset);

        public void SortPricing()
        {
            _bids = _bids.OrderByDescending(x => x.Price).ToList();
            _asks = _asks.OrderBy(x => x.Price).ToList();
        }

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

        private OrderBook _reversed;
        public OrderBook Reversed => _reversed ?? (_reversed = GetReversed());

        public bool IsReversed { get; private set; }

        private OrderBook GetReversed()
        {
            return new OrderBook(Network, Pair.Reversed, _bids.Select(x => x.Reverse(Pair.Asset1)), _asks.Select(x => x.Reverse(Pair.Asset1)))
            {
                _reversed = this,
                IsReversed = true
            };
        }

        public int Count => Asks.Count + Bids.Count;

        public OrderBook AsPair(AssetPair pair)
        {
            return pair.Id == Pair.Id ? this : Reversed;
        }

        public override string ToString()
        {
            return Network.Name + " " + Pair;
        }
    }
}
