﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using LiteDB;
using Prime.Core.Exchange;
using RestEase;
using Prime.Utility;
using OrderBook = Prime.Core.OrderBook;

#endregion

namespace plugins
{
    public class CoinbaseProvider : IExchangeProvider, IWalletService, IOrderBookProvider, IOhlcProvider
    {
        private static readonly ObjectId IdHash = "prime:coinbase".GetObjectIdHashCode();

        private const string CoinbaseApiVersion = "v2";
        private const string CoinbaseApiUrl = "https://api.coinbase.com/" + CoinbaseApiVersion + "/";

        private const string GdaxApiUrl = "https://api.gdax.com";

        public Network Network { get; } = new Network("Coinbase");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<T>(CoinbaseApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            // var key = context.GetKey(this);
            // TODO: DELETE
            var key = new ApiKey(Network, "", "", "");

            return RestClient.For<T>(CoinbaseApiUrl, new CoinbaseAuthenticator(key).GetRequestModifier) as T;
        }

        private IGdaxApi GetGdaxApi()
        {
            return RestClient.For<IGdaxApi>(GdaxApiUrl);
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);
            var pairCode = GetCoinbaseTicker(context.Pair.Asset1, context.Pair.Asset2);
            var r = await api.GetLatestPrice(pairCode);

            var price = new LatestPrice(new Money(r.data.amount, r.data.currency.ToAsset(this)))
            {
                BaseAsset = context.Pair.Asset1
            };

            return price;
        }

        public async Task<LatestPrices> GetLatestPricesAsync(PublicPricesContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);

            var prices = new List<Money>();

            foreach (var asset in context.Assets)
            {
                var pairCode = GetCoinbaseTicker(context.BaseAsset, asset);
                var r = await api.GetLatestPrice(pairCode);

                prices.Add(new Money(r.data.amount, r.data.currency.ToAsset(this)));
            }

            var latestPrices = new LatestPrices()
            {
                BaseAsset = context.BaseAsset,
                Prices = prices
            };

            return latestPrices;
        }

        private string GetCoinbaseTicker(Asset baseAsset, Asset asset)
        {
            return new AssetPair(baseAsset.ToRemoteCode(this), asset.ToRemoteCode(this)).TickerDash();
        }

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
            var api = GetGdaxApi();
            var r = await api.GetProducts();

            var pairs = new AssetPairs();

            foreach (var rProduct in r)
            {
                pairs.Add(new AssetPair(rProduct.base_currency, rProduct.quote_currency));
            }

            return pairs;
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);
            var r = await api.GetAccountsAsync();
            return r != null;
        }

        public bool CanMultiDepositAddress { get; } = true;

        public bool CanGenerateDepositAddress { get; } = true;

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);
            var r = await api.GetAccountsAsync();

            var results = new BalanceResults(this);

            foreach (var a in r.data)
            {
                if (a.balance == null)
                    continue;

                var c = a.balance.currency.ToAsset(this);
                results.AddBalance(c, a.balance.amount);
                results.AddAvailable(c, a.balance.amount);
                results.AddReserved(c, a.balance.amount);
            }

            return results;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);

            var accid = "";

            var accs = await api.GetAccounts();
            var ast = context.Asset.ToRemoteCode(this);

            var acc = accs.data.FirstOrDefault(x => string.Equals(x.currency, ast, StringComparison.OrdinalIgnoreCase));
            if (acc == null)
                return null;

            accid = acc.id;

            if (accid == null)
                return null;

            var r = await api.GetAddressesAsync(acc.id);

            if (r.data.Count == 0 && context.CanGenerateAddress)
            {
                var cr = await api.CreateAddressAsync(accid);
                if (cr != null)
                    r.data.AddRange(cr.data);
            }

            var addresses = new WalletAddresses();

            foreach (var a in r.data)
            {
                if (string.IsNullOrWhiteSpace(a.address))
                    continue;

                var forasset = FromNetwork(a.network);
                if (!context.Asset.Equals(forasset))
                    continue;

                addresses.Add(new WalletAddress(this, context.Asset) { Address = a.address });
            }

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);
            var accs = await api.GetAccounts();
            var addresses = new WalletAddresses();

            var accountIds = accs.data.Select(x => new KeyValuePair<string, string>(x.currency, x.id));

            foreach (var kvp in accountIds)
            {
                var r = await api.GetAddressesAsync(kvp.Value);

                foreach (var rAddress in r.data)
                {
                    if(string.IsNullOrWhiteSpace(rAddress.address))
                        continue;

                    addresses.Add(new WalletAddress(this, kvp.Key.ToAsset(this))
                    {
                        Address = rAddress.address
                    });
                }
            }

            return addresses;
        }

        private Asset FromNetwork(string network)
        {
            switch (network)
            {
                case "bitcoin":
                    return "BTC".ToAssetRaw();
                case "litecoin":
                    return "LTC".ToAssetRaw();
                case "ethereum":
                    return "ETH".ToAssetRaw();
                default:
                    return Asset.None;
            }
        }

        public async Task<OrderBook> GetOrderBook(OrderBookContext context)
        {
            var api = GetGdaxApi();
            var pairCode = context.Pair.TickerDash();

            // TODO: Check this! Can we use limit when we query all records?
            var recordsLimit = 1000;

            var r = await api.GetProductOrderBook(pairCode, OrderBookDepthLevel.FullNonAggregated);

            var bids = context.MaxRecordsCount.HasValue 
                ? r.bids.Take(context.MaxRecordsCount.Value / 2).ToArray() 
                : r.bids.Take(recordsLimit).ToArray();
            var asks = context.MaxRecordsCount.HasValue
                ? r.asks.Take(context.MaxRecordsCount.Value / 2).ToArray()
                : r.asks.Take(recordsLimit).ToArray();

            var orderBook = new OrderBook();

            foreach (var rBid in bids)
            {
                var bid = ConvertToOrderBookRecord(rBid);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(bid.Price, context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = bid.Size
                    },
                    Type = OrderBookType.Bid
                });
            }

            foreach (var rAsk in asks)
            {
                var ask = ConvertToOrderBookRecord(rAsk);

                orderBook.Add(new OrderBookRecord()
                {
                    Data = new BidAskData()
                    {
                        Price = new Money(ask.Price, context.Pair.Asset2),
                        Time = DateTime.UtcNow,
                        Volume = ask.Size
                    },
                    Type = OrderBookType.Ask
                });
            }

            return orderBook;
        }

        private (decimal Price, decimal Size) ConvertToOrderBookRecord(string[] data)
        {
            if(!decimal.TryParse(data[0], out var price) || !decimal.TryParse(data[1], out var size))
                throw new ApiResponseException("API returned incorrect format of price data", this);

            return (price, size);
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = GetGdaxApi();
            var currencyCode = context.Pair.TickerDash();

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcResolutionAdapter.GetHash(context.Pair, context.Market, Network);

            var granularitySeconds = GetSeconds(context.Market);
            var maxNumberOfCandles = 200;

            var tsFrom = (long)context.Range.UtcFrom.ToUnixTimeStamp();
            var tsTo = (long)context.Range.UtcTo.ToUnixTimeStamp();
            var tsStep = maxNumberOfCandles * granularitySeconds;

            var currTsTo = tsTo;
            var currTsFrom = tsTo - tsStep;

            while (currTsTo > tsFrom)
            {
                var candles = await api.GetCandles(currencyCode, currTsFrom.ToUtcDateTime(), currTsTo.ToUtcDateTime(), granularitySeconds);

                foreach (var candle in candles)
                {
                    var dateTime = ((long)candle[0]).ToUtcDateTime();
                    ohlc.Add(new OhlcEntry(seriesId, dateTime, this)
                    {
                        Low = (double)candle[1],
                        High = (double)candle[2],
                        Open = (double)candle[3],
                        Close = (double)candle[4],
                        VolumeTo = (long)candle[5],
                        VolumeFrom = (long)candle[5],
                        WeightedAverage = 0 // Is not provided by API.
                    });
                }

                currTsTo = currTsFrom;

                if (currTsTo - tsStep >= tsFrom)
                    currTsFrom -= tsStep;
                else
                    currTsFrom = tsFrom;

                // TODO: do something to ged rid of Thread.Sleep.
                Thread.Sleep(500);
            }

            return ohlc;
        }

        private int GetSeconds(TimeResolution market)
        {
            switch (market)
            {
                case TimeResolution.Second:
                    return 1;
                case TimeResolution.Minute:
                    return 60;
                case TimeResolution.Hour:
                    return 3600;
                case TimeResolution.Day:
                    return 62400;
                default:
                    throw new ArgumentOutOfRangeException(nameof(market), market, null);
            }
        }
    }
}