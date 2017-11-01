using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Plugins.Services.Base;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Bittrex
{
    public class BittrexProvider : IExchangeProvider, IWalletService, IOrderBookProvider, IPublicPricesProvider
    {
        private const string BittrexApiVersion = "v1.1";
        private const string BittrexApiUrl = "https://bittrex.com/api/" + BittrexApiVersion;

        private static readonly ObjectId IdHash = "prime:bittrex".GetObjectIdHashCode();

        // No information in API documents.
        // https://bitcoin.stackexchange.com/questions/53778/bittrex-api-rate-limit
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(60, 1);

        private RestApiClientProvider<IBittrexApi> ApiProvider { get; }

        public Network Network { get; } = new Network("Bittrex");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;
        public bool CanMultiDepositAddress => false;

        /// <summary>
        /// Only allows new address generating if it is empty. Otherwise only peeking.
        /// </summary>
        public bool CanGenerateDepositAddress => true;

        public bool CanPeekDepositAddress => false;
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BittrexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBittrexApi>(BittrexApiUrl, this, k => new BittrexAuthenticator(k).GetRequestModifier);
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetAllBalances();

            return r != null && r.success && r.result != null;
        }

        public async Task<LatestPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.TickerDash();
            var r = await api.GetTicker(pairCode);

            CheckResponseErrors(r);

            return new LatestPrice(new Money(1 / r.result.Last, context.Pair.Asset2), context.Pair.Asset1);
        }

        public async Task<List<LatestPrice>> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return await GetPricesAsync(context);
        }

        public async Task<List<LatestPrice>> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarketSummaries();

            CheckResponseErrors(r);

            var prices = new List<LatestPrice>();

            foreach (var pair in context.Pairs)
            {
                var pairCode = pair.TickerDash();

                var ms = r.result.FirstOrDefault(x => x.MarketName.Equals(pairCode));

                if(ms == null)
                    throw new ApiResponseException("No price returned for selected currency", this);

                prices.Add(new LatestPrice(pair, 1 / ms.Last));
            }

            return prices;
        }


        public bool DoesMultiplePairs => false;

        public bool PricesAsAssetQuotes => false;

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarkets();

            CheckResponseErrors(r);

            var pairs = new AssetPairs();

            foreach (var rEntry in r.result)
            {
                var pair = new AssetPair(rEntry.BaseCurrency, rEntry.MarketCurrency);
                pairs.Add(pair);
            }

            return pairs;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllBalances();
            CheckResponseErrors(r);

            var balances = new BalanceResults();

            foreach (var rBalance in r.result)
            {
                var asset = rBalance.Currency.ToAsset(this);

                balances.Add(new BalanceResult(asset)
                {
                    Available = new Money(rBalance.Available, asset),
                    Balance = new Money(rBalance.Balance, asset),
                    Reserved = new Money(rBalance.Pending, asset)
                });
            }

            return balances;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllBalances();
            CheckResponseErrors(r);

            var addresses = new WalletAddresses();

            foreach (var rBalance in r.result)
            {
                addresses.Add(new WalletAddress(this, rBalance.Currency.ToAsset(this))
                {
                    Address = rBalance.CryptoAddress
                });
            }

            return addresses;
        }

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAllBalances();

            var addresses = new WalletAddresses();

            var address = r.result.FirstOrDefault(x => x.Currency.ToAsset(this).Equals(context.Asset));

            if (address != null)
            {
                addresses.Add(new WalletAddress(this, context.Asset)
                {
                    Address = address.CryptoAddress
                });
            }

            return addresses;

            // TODO: re-implement.
            //if (context.CanGenerateAddress)
            //{
            //    var r = await api.GetDepositAddress(context.Asset.ToRemoteCode(this));
            //    var addresses = new WalletAddresses();

            //    if (r.success)
            //    {
            //        addresses.Add(new WalletAddress(this, context.Asset)
            //        {
            //            Address = r.result.Address
            //        });
            //    }
            //    else
            //    {
            //        if (String.IsNullOrEmpty(r.message) == false && r.message.Contains(ErrorText_AddressIsGenerating) == false)
            //        {
            //            throw new ApiResponseException("Unexpected error during address generation", this);
            //        }
            //    }

            //    return addresses;
            //}
            //else
            //{
            //    var r = await api.GetAllBalances();

            //    var addresses = new WalletAddresses();

            //    var address = r.result.FirstOrDefault(x => x.Currency.ToAsset(this).Equals(context.Asset));

            //    if (address != null)
            //    {
            //        addresses.Add(new WalletAddress(this, context.Asset)
            //        {
            //            Address = address.CryptoAddress
            //        });
            //    }

            //    return addresses;
            //}
        }

        private void CheckResponseErrors<T>(BittrexSchema.BaseResponse<T> response)
        {
            if (response.success == false)
                throw new ApiResponseException($"API error: {response.message}", this);
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = context.Pair.TickerDash();

            var r = await api.GetOrderBook(pairCode);

            CheckResponseErrors(r);

            var orderBook = new OrderBook();

            var buys = context.MaxRecordsCount.HasValue
                ? r.result.buy.Take(context.MaxRecordsCount.Value / 2)
                : r.result.buy;
            var sells = context.MaxRecordsCount.HasValue
                ? r.result.sell.Take(context.MaxRecordsCount.Value / 2)
                : r.result.sell;

            foreach (var rBuy in buys)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Price = new Money(1 / rBuy.Rate, context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = rBuy.Quantity
                    }
                });
            }

            foreach (var rSell in sells)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Price = new Money(1 / rSell.Rate, context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = rSell.Quantity
                    }
                });
            }

            return orderBook;
        }
    }
}