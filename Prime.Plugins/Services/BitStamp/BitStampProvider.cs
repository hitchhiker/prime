using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Utility;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common.Exchange;
using Prime.Plugins.Services.Base;
using RestEase;

namespace Prime.Plugins.Services.BitStamp
{
    public class BitStampProvider : IExchangeProvider, IWalletService, IApiProvider, IOrderBookProvider
    {
        private const string BitStampApiUrl = "https://www.bitstamp.net/api/";
        public const string BitStampApiVersion = "v2";

        public Network Network { get; } = new Network("BitStamp");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:bitstamp".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IBitStampApi>(BitStampApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);

            return RestClient.For<IBitStampApi>(BitStampApiUrl, new BitStampAuthenticator(key).GetRequestModifier) as T;
        }

        public Task<bool> TestApiAsync(ApiTestContext context)
        {
            var t = new Task<bool>(() =>
            {
                var api = GetApi<IBitStampApi>(context);
                var r = api.GetAccountBalances().Result;

                return r != null;
            });
            t.Start();
            return t;
        }

        public ApiConfiguration GetApiConfiguration => new ApiConfiguration()
        {
            HasSecret = true,
            HasExtra = true,
            ApiExtraName = "Customer Number"
        };

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IBitStampApi>(context);

            var r = await api.GetTicker(context.Pair.TickerSimple());

            var latestPrice = new LatestPrice()
            {
                Price = new Money(r.last, context.Pair.Asset2),
                BaseAsset = context.Pair.Asset1,
                UtcCreated = r.timestamp.ToUtcDateTime()
            };

            return latestPrice;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            var api = GetApi<IBitStampApi>(context);

            var moneyList = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var pairCode = context.BaseAsset.ToPair(asset).TickerSimple();
                var r = await api.GetTicker(pairCode);

                moneyList.Add(new Money(r.last, asset));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                Prices = moneyList,
                UtcCreated = DateTime.UtcNow
            };

            return latestPrices;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        private static readonly string _pairs = "btcusd,btceur,eurusd,xrpusd,xrpeur,xrpbtc,ltcusd,ltceur,ltcbtc,ethusd,etheur,ethbtc";

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() => Pairs);
            t.RunSynchronously();
            return t;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = GetApi<IBitStampApi>(context);

            var r = await api.GetAccountBalances();

            var balances = new BalanceResults(this);

            var btcAsset = "btc".ToAsset(this);
            var usdAsset = "usd".ToAsset(this);
            var eurAsset = "eur".ToAsset(this);

            balances.Add(new BalanceResult(btcAsset)
            {
                Available = new Money(r.btc_available, btcAsset),
                Balance = new Money(r.btc_balance, btcAsset),
                Reserved = new Money(r.btc_reserved, btcAsset)
            });

            balances.Add(new BalanceResult(usdAsset)
            {
                Available = new Money(r.usd_available, usdAsset),
                Balance = new Money(r.usd_balance, usdAsset),
                Reserved = new Money(r.usd_reserved, usdAsset)
            });

            balances.Add(new BalanceResult(eurAsset)
            {
                Available = new Money(r.eur_available, eurAsset),
                Balance = new Money(r.eur_reserved, eurAsset),
                Reserved = new Money(r.eur_balance, eurAsset)
            });

            return balances;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool CanMultiDepositAddress => true;

        public bool CanGenerateDepositAddress => false;

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var addresses = new WalletAddresses();
            var wac = new WalletAddressAssetContext("ETH".ToAsset(this), context.CanGenerateAddress, context.UserContext, context.L);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));

            wac.Asset = "BTC".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));

            wac.Asset = "XRP".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));

            wac.Asset = "LTC".ToAsset(this);
            addresses.AddRange(await GetAddressesForAssetAsync(wac));

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = GetApi<IBitStampApi>(context);
            var currencyPath = GetCurrencyPath(context.Asset);

            var r = await api.GetDepositAddress(currencyPath);

            var processedAddress = ProcessAddressResponce(context.Asset, r);

            if (!this.ExchangeHas(context.Asset))
                return null;

            var walletAddress = new WalletAddress(this, context.Asset)
            {
                Address = processedAddress
            };

            var addresses = new WalletAddresses(walletAddress);

            return addresses;
        }

        private string ProcessAddressResponce(Asset asset, string response)
        {
            switch (asset.ShortCode)
            {
                case "BTC":
                    return response.Trim('\"');
                case "XRP":
                    var splitted = JsonConvert.DeserializeObject<BitStampSchema.AccountAddressResponse>(response).address.Split(new[] { "?dt" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length < 1)
                        throw new ApiResponseException("XRP address has incorrect format", this);
                    return splitted[0];
                case "ETH":
                case "LTC":
                    var addrr = JsonConvert.DeserializeObject<BitStampSchema.AccountAddressResponse>(response);
                    return addrr.address;
                default:
                    throw new NullReferenceException("No deposit address for specified currency");
            }
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = GetApi<IBitStampApi>(context);
            var pairCode = GetBitStampTicker(context.Pair);

            var r = await api.GetOrderBook(pairCode);
            var orderBook = new OrderBook();

            var date = r.timestamp.ToUtcDateTime();

            var asks = context.MaxRecordsCount.HasValue ? r.asks.Take(context.MaxRecordsCount.Value / 2) : r.asks;
            var bids = context.MaxRecordsCount.HasValue ? r.bids.Take(context.MaxRecordsCount.Value / 2) : r.bids;

            foreach (var rAsk in asks)
            {
                var data = GetBidAskData(rAsk);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(data.Price, context.Pair.Asset2),
                        Volume = data.Amount,
                        Time = date
                    },
                    Type = OrderBookType.Ask
                });
            }

            foreach (var rBid in bids)
            {
                var data = GetBidAskData(rBid);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(data.Price, context.Pair.Asset2),
                        Volume = data.Amount,
                        Time = date
                    },
                    Type = OrderBookType.Bid
                });
            }

            return orderBook;
        }

        private (decimal Price, decimal Amount) GetBidAskData(decimal[] data)
        {
            decimal price = 0;
            decimal amount = 0;

            price = data[0];
            amount = data[1];

            return (price, amount);
        }

        private string GetBitStampTicker(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this).ToLower()}{pair.Asset2.ToRemoteCode(this).ToLower()}";
        }

        private string GetCurrencyPath(Asset asset)
        {
            switch (asset.ShortCode)
            {
                case "LTC":
                    return BitStampApiVersion + "/ltc_address";
                case "ETH":
                    return BitStampApiVersion + "/eth_address";
                case "BTC":
                    return "bitcoin_deposit_address";
                case "XRP":
                    return "ripple_address";
                default:
                    throw new NullReferenceException("No deposit address for specified currency");
            }
        }
    }
}
