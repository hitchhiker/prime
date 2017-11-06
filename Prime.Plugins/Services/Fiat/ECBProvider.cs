using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Fiat
{
    public class EcbProvider : IPublicPricesProvider
    {
        private static readonly ObjectId IdHash = "prime:ECB:PROVIDER".GetObjectIdHashCode();
        private static readonly Network _network = Networks.I.Get("ECB (Fiat)");
        private static readonly IRateLimiter _limiter = new NoRateLimits();
        private static readonly string _title = "European Central Bank";

        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => _limiter;
        public bool IsDirect => true;

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            throw new NotImplementedException();
        }

        public Network Network => _network;
        public bool Disabled => false;
        public int Priority => 1;
        public string AggregatorName => null;
        public string Title => _title;

        private DateTime _date;
        private DateTime _lastRefresh;
        private Dictionary<AssetPair, decimal> _lastRates = new Dictionary<AssetPair, decimal>();
        private readonly UniqueList<AssetPair> _pairs = new UniqueList<AssetPair>();
        private readonly object _lock = new object();

        public static Asset Euro = "EUR".ToAssetRaw();

        public async Task<Dictionary<AssetPair, decimal>> GetRatesAsync()
        {
            return await new Task<Dictionary<AssetPair, decimal>>(() =>
            {
                lock (_lock)
                {
                    if (_lastRefresh.IsWithinTheLast(TimeSpan.FromMinutes(1)))
                        return _lastRates;
                    return _lastRates = GetXmlRates();
                }
            });
        }

        private Dictionary<AssetPair, decimal> GetXmlRates()
        {
            var returnList = new Dictionary<AssetPair, decimal>();

            var date = string.Empty;
            using (var xmlr = XmlReader.Create("http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"))
            {
                xmlr.ReadToFollowing("Cube");
                while (xmlr.Read())
                {
                    if (xmlr.NodeType != XmlNodeType.Element)
                        continue;

                    if (xmlr.GetAttribute("time") != null)
                        date = xmlr.GetAttribute("time");
                    else
                    {
                        var pair = new AssetPair(xmlr.GetAttribute("currency").ToAssetRaw(), Euro);
                        returnList.Add(pair, decimal.Parse(xmlr.GetAttribute("rate"), CultureInfo.InvariantCulture));
                    }
                }

                _date = DateTime.Parse(date);
            }
            _lastRefresh = DateTime.UtcNow;
            return returnList;
        }

        public async Task<decimal> GetMultiplierAsync(AssetPair pair)
        {
            var rates = await GetRatesAsync();
            return rates.Get(pair, 0);
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            if (_pairs.Any())
                return new AssetPairs(_pairs);

            var rates = await GetRatesAsync();

            lock (_lock)
            {
                var pairs = new AssetPairs(rates.Select(x => x.Key));
                _pairs.Clear();
                _pairs.AddRange(pairs);
                return pairs;
            }
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var rates = await GetRatesAsync();

            var lp = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var rate = rates.FirstOrDefault(x => Equals(x.Key, pair));
                if (rate.Key == null)
                    continue;

                lp.MarketPrices.Add(new MarketPrice(rate.Key, rate.Value));
            }

            return lp;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var r = await GetPricesAsync(new PublicPricesContext(new List<AssetPair>() {context.Pair}, context.L));
            return r.MarketPrices.FirstOrDefault();
        }
    }
}
