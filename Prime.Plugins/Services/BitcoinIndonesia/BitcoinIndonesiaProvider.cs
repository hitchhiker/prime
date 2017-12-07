using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitcoinIndonesia
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://vip.bitcoin.co.id/downloads/BITCOINCOID-API-DOCUMENTATION.pdf
    public class BitcoinIndonesiaProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string BitcoinIndonesiaApiUrl = "https://vip.bitcoin.co.id/api/";

        private static readonly ObjectId IdHash = "prime:bitcoinindonesia".GetObjectIdHashCode();

        //To prevent abusive requests, we limit API call to 180 requests per minute.
        //https://vip.bitcoin.co.id/downloads/BITCOINCOID-API-DOCUMENTATION.pdf
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(180, 1);

        private RestApiClientProvider<IBitcoinIndonesiaApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("BitcoinIndonesia");
        private const string PairsCsv = "BTC_IDR,XLM_IDR,NXT_IDR,XLM_BTC,BCH_IDR,NXT_BTC,BTG_IDR,XRP_IDR,ETH_IDR,ETC_IDR,WAVES_IDR,XZC_IDR,ETH_BTC,XRP_BTC,BTS_BTC,LTC_IDR,DOGE_BTC,DRK_BTC,LTC_BTC,NEM_BTC";
        
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;
        public char? CommonPairSeparator => '_';

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        //Asset pairs obtained from here and tested one-by-one as not all the ones in the list work: https://cryptocoincharts.info/markets/show/bitcoin-co-id
        public AssetPairs _pairs;
        public AssetPairs Pairs => _pairs ?? (_pairs = new AssetPairs(PairsCsv.ToCsv().Select(x => x.ToAssetPairRaw())));

        public BitcoinIndonesiaProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitcoinIndonesiaApi>(BitcoinIndonesiaApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetTickerAsync("btc_idr").ConfigureAwait(false);

            return r?.ticker != null && r.ticker.Count > 0;
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        private BitcoinIndonesiaSchema.TickerEntryResponse CreateTickerObject(Dictionary<string, decimal> response, AssetPair pair)
        {
            response.TryGetValue("high", out decimal high);
            response.TryGetValue("low", out decimal low);
            response.TryGetValue("vol_" + pair.Asset1.ShortCode.ToLower(), out decimal volBase);
            response.TryGetValue("vol_" + pair.Asset2.ShortCode.ToLower(), out decimal volQuote);
            response.TryGetValue("last", out decimal last);
            response.TryGetValue("buy", out decimal buy);
            response.TryGetValue("sell", out decimal sell);
            response.TryGetValue("server_time", out decimal server_time);

            return new BitcoinIndonesiaSchema.TickerEntryResponse()
            {
                high = high,
                low = low,
                vol_base = volBase,
                vol_quote = volQuote,
                last = last,
                buy = buy,
                sell = sell,
                server_time = (long)server_time
            };
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true }
        };

        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this).ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            if (r?.ticker == null || r.ticker.Count == 0)
            {
                throw new ApiResponseException("No ticker returned", this);
            }

            var ticker = CreateTickerObject(r.ticker, context.Pair);

            if (ticker == null)
            {
                throw new ApiResponseException("Ticker response was not converted successfully", this);
            }

            return new MarketPrices(new MarketPrice(Network, context.Pair, ticker.last)
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, ticker.sell, ticker.buy, ticker.low, ticker.high),
                Volume = new NetworkPairVolume(Network, context.Pair, ticker.vol_base, ticker.vol_quote)
            });
        }
    }
}
