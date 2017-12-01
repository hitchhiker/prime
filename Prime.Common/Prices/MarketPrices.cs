using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Utility;
using LiteDB;
using System.Collections;

namespace Prime.Common
{
    public class MarketPrices : ModelBase, ISerialiseAsObject, IReadOnlyList<MarketPrice>
    {
        public MarketPrices() { }

        public MarketPrices(MarketPrice price)
        {
            Add(price);

            WasViaSingleMethod = true;
            UtcCreated = price.UtcCreated;
        }

        public MarketPrices(IEnumerable<MarketPrices> prices) : this(prices?.Where(x=>x!=null).SelectMany(x=>x.PricesList)) { }

        public MarketPrices(IEnumerable<MarketPrice> prices)
        {
            AddRange(prices);
            UtcCreated = DateTime.UtcNow;
        }

        public void Add(MarketPrice price)
        {
            if (price?.Price > 0)
                PricesList.Add(price);
            else if (price?.Pair != null)
                MissedPairs.Add(price?.Pair);
        }

        public void AddRange(IEnumerable<MarketPrice> prices)
        {
            if (prices == null)
                return;

            foreach (var i in prices)
                Add(i);
        }

        public override ObjectId GenerateId()
        {
            if (!PricesList.Any())
                throw new Exception("Cannot auto-generate an ID for 'MarketPrices' unless it has prices");

            if (PricesList.GroupBy(x => x.Network).Count() > 1)
                throw new Exception("Cannot auto-generate an ID for 'MarketPrices' unless all the prices are from the same network");

            return GetHash(FirstPrice.Network);
        }

        public static ObjectId GetHash(Network network)
        {
            return ("latestpricenetworkdata:" + network.Id).GetObjectIdHashCode();
        }

        public MarketPrice this[int index] => ((IReadOnlyList<MarketPrice>)PricesList)[index];

        [Bson]
        private List<MarketPrice> PricesList { get; set; } = new List<MarketPrice>();

        [Bson]
        public AssetPairs MissedPairs { get; set; } = new AssetPairs();

        [Bson]
        public bool WasViaSingleMethod { get; set; }

        [Bson]
        public DateTime UtcCreated { get; set; }

        public MarketPrice FirstPrice => PricesList.FirstOrDefault();

        public bool IsCompleted => MissedPairs.Count == 0;

        public int Count => ((IReadOnlyList<MarketPrice>)PricesList).Count;

        public IEnumerator<MarketPrice> GetEnumerator() => ((IReadOnlyList<MarketPrice>)PricesList).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<MarketPrice>)PricesList).GetEnumerator();

        public MarketPrices AsNormalised()
        {
            var normalised = PricesList.ToList();
            
            var forNormalisation = normalised.Where(x => !x.Pair.IsNormalised).ToList();
            normalised.RemoveAll(x => forNormalisation.Contains(x));

            foreach (var i in forNormalisation)
                normalised.Add(i.Reversed);

            return new MarketPrices(normalised) { IdBacking = IdBacking, UtcCreated = UtcCreated, MissedPairs = new AssetPairs(MissedPairs.Select(x=>x.Normalised))};
        }
    }
}
