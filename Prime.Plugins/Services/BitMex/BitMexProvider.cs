using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Common.Wallet.Withdrawal;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;
using Prime.Plugins.Services.Base;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    public class BitMexProvider :
        IBalanceProvider, IOhlcProvider, IOrderBookProvider, IPublicPricesProvider,
        IWithdrawalPlacementProviderExtended, IWithdrawalHistoryProvider, IWithdrawalCancelationProvider, IWithdrawalConfirmationProvider
    {
        private static readonly ObjectId IdHash = "prime:bitmex".GetObjectIdHashCode();

        private const String BitMexApiUrl = "https://www.bitmex.com/api/v1";
        private const String BitMexTestApiUrl = "https://testnet.bitmex.com/api/v1";

        private static readonly string _pairs = "btcusd";
        private const decimal ConversionRate = 0.00000001m;

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);

        private RestApiClientProvider<IBitMexApi> ApiProvider { get; }
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(150, 5, 300, 5);

        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;
        
        public bool CanGenerateDepositAddress => false;
        public bool CanPeekDepositAddress => false;
        public ObjectId Id => IdHash;
        public Network Network => Networks.I.Get("BitMex");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;

        public BitMexProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitMexApi>(BitMexApiUrl, this, (k) => new BitMexAuthenticator(k).GetRequestModifier);
        }

        public Task<bool> TestPublicApiAsync()
        {
            var t = new Task<bool>(() => true);
            t.Start();
            return t;
        }

        private string ConvertToBitMexInterval(TimeResolution market)
        {
            switch (market)
            {
                case TimeResolution.Minute:
                    return "1m";
                case TimeResolution.Hour:
                    return "1h";
                case TimeResolution.Day:
                    return "1d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(market), market, null);
            }
        }

        public async Task<OhlcData> GetOhlcAsync(OhlcContext context)
        {
            var api = ApiProvider.GetApi(context);//  GetApi<IBitMexApi>(context);

            var resolution = ConvertToBitMexInterval(context.Market);
            var startDate = context.Range.UtcFrom;
            var endDate = context.Range.UtcTo;

            var r = await api.GetTradeHistory(context.Pair.Asset1.ToRemoteCode(this), resolution, startDate, endDate).ConfigureAwait(false);

            var ohlc = new OhlcData(context.Market);
            var seriesId = OhlcUtilities.GetHash(context.Pair, context.Market, Network);

            foreach (var instrActive in r)
            {
                ohlc.Add(new OhlcEntry(seriesId, instrActive.timestamp, this)
                {
                    Open = (double)instrActive.open,
                    Close = (double)instrActive.close,
                    Low = (double)instrActive.low,
                    High = (double)instrActive.high,
                    VolumeTo = (long)instrActive.volume,
                    VolumeFrom = (long)instrActive.volume,
                    WeightedAverage = (double)(instrActive.vwap ?? 0) // BUG: what to set if vwap is NULL?
                });
            }

            return ohlc;
        }

        private static readonly IAssetCodeConverter AssetCodeConverter = new BitMexCodeConverter();
        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return AssetCodeConverter;
        }

        public async Task<MarketPrice> GetPriceAsync(PublicPriceContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetLatestPriceAsync(context.Pair.Asset1.ToRemoteCode(this)).ConfigureAwait(false);

            var rPrice = r.FirstOrDefault();

            if (rPrice == null || rPrice.lastPrice.HasValue == false)
                throw new ApiResponseException($"Specified currency pair {context.Pair} is not supported by provider", this);

            return new MarketPrice(context.Pair, rPrice.lastPrice.Value);
        }

        public Task<MarketPricesResult> GetAssetPricesAsync(PublicAssetPricesContext context)
        {
            return GetPricesAsync(context);
        }

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetLatestPricesAsync().ConfigureAwait(false);

            var prices = new MarketPricesResult();

            foreach (var pair in context.Pairs)
            {
                var pairCode = GetBitMexTicker(pair);

                var data = r.FirstOrDefault(x =>
                    x.symbol.Equals(pairCode)
                );

                if (data == null || data.lastPrice.HasValue == false)
                {
                    prices.MissedPairs.Add(pair);
                    continue;
                }

                prices.MarketPrices.Add(new MarketPrice(pair, data.lastPrice.Value));
            }

            return prices;
        }


        public BuyResult Buy(BuyContext ctx)
        {
            throw new NotImplementedException();
        }

        public SellResult Sell(SellContext ctx)
        {
            throw new NotImplementedException();
        }

        public Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            return Task.Run(() => Pairs);
        }

        public async Task<bool> TestPrivateApiAsync(ApiPrivateTestContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetUserInfoAsync().ConfigureAwait(false);
            return r != null;
        }

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = ApiProvider.GetApi(context);

            var remoteAssetCode = context.Asset.ToRemoteCode(this);
            var depositAddress = await api.GetUserDepositAddressAsync(remoteAssetCode).ConfigureAwait(false);

            depositAddress = depositAddress.Trim('\"');

            var addresses = new WalletAddresses();
            var walletAddress = new WalletAddress(this, context.Asset) { Address = depositAddress };

            addresses.Add(walletAddress);

            return addresses;
        }

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = ApiProvider.GetApi(context);
            var addresses = new WalletAddresses();

            foreach (var assetPair in Pairs)
            {
                var adjustedCode = AdjustAssetCode(assetPair.Asset1.ShortCode);

                var depositAddress = await api.GetUserDepositAddressAsync(adjustedCode).ConfigureAwait(false);

                depositAddress = depositAddress.Trim('\"');

                // BUG: how to convert XBt from Pairs to BTC?
                addresses.Add(new WalletAddress(this, Asset.Btc)
                {
                    Address = depositAddress
                });
            }

            return addresses;
        }

        public Task<bool> CreateAddressForAssetAsync(WalletAddressAssetContext context)
        {
            throw new NotImplementedException();
        }

        [Obsolete] // BUG: review.
        private string AdjustAssetCode(string input)
        {
            // TODO: should be re-factored.
            var config = new Dictionary<string, string>();

            config.Add("XBT", "XBt");

            return config.ContainsKey(input) ? config[input] : null;
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetUserWalletInfoAsync("XBt").ConfigureAwait(false);

            var results = new BalanceResults(this);

            var btcAmount = (decimal)ConversionRate * r.amount;

            var c = Asset.Btc;

            var balance = new BalanceResult(c);
            balance.Balance = new Money(btcAmount, c);
            balance.Available = new Money(btcAmount, c);
            balance.Reserved = new Money(0, c);

            results.Add(balance);

            return results;
        }

        public async Task<OrderBook> GetOrderBookAsync(OrderBookContext context)
        {
            var api = ApiProvider.GetApi(context);

            var pairCode = GetBitMexTicker(context.Pair);

            var r = context.MaxRecordsCount.HasValue
                ? await api.GetOrderBookAsync(pairCode, context.MaxRecordsCount.Value).ConfigureAwait(false)
                : await api.GetOrderBookAsync(pairCode, 0).ConfigureAwait(false);

            var buyAction = "buy";
            var sellAction = "sell";

            var buys = context.MaxRecordsCount.HasValue
                ? r.Where(x => x.side.ToLower().Equals(buyAction)).OrderBy(x => x.id)
                    .Take(context.MaxRecordsCount.Value / 2).ToList()
                : r.Where(x => x.side.ToLower().Equals(buyAction)).OrderBy(x => x.id).ToList();

            var sells = context.MaxRecordsCount.HasValue
                ? r.Where(x => x.side.ToLower().Equals(sellAction)).OrderBy(x => x.id)
                    .Take(context.MaxRecordsCount.Value / 2).ToList()
                : r.Where(x => x.side.ToLower().Equals(sellAction)).OrderBy(x => x.id).ToList();

            var orderBook = new OrderBook();

            foreach (var buy in buys)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Bid,
                    Data = new BidAskData()
                    {
                        Price = new Money(buy.price, context.Pair.Asset2),
                        Time = DateTime.Now, // Since it returns current state of OrderBook, date time is set to Now.
                        Volume = buy.size
                    }
                });
            }

            foreach (var sell in sells)
            {
                orderBook.Add(new OrderBookRecord()
                {
                    Type = OrderBookType.Ask,
                    Data = new BidAskData()
                    {
                        Price = new Money(sell.price, context.Pair.Asset2),
                        Time = DateTime.Now,
                        Volume = sell.size
                    }
                });
            }

            return orderBook;
        }

        private string GetBitMexTicker(AssetPair pair)
        {
            return $"{pair.Asset1.ToRemoteCode(this)}{pair.Asset2.ToRemoteCode(this)}".ToUpper();
        }

        public bool IsFeeIncluded => false;

        public async Task<WithdrawalPlacementResult> PlaceWithdrawal(WithdrawalPlacementContextExtended context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>();

            if (!String.IsNullOrWhiteSpace(context.AuthenticationToken))
                body.Add("otpToken", context.AuthenticationToken);

            body.Add("currency", context.Price.Asset.ToRemoteCode(this));
            body.Add("amount", context.Price.ToDecimalValue() / ConversionRate);
            body.Add("address", context.Address);
            body.Add("fee", context.CustomFee.ToDecimalValue() / ConversionRate);

            var r = await api.RequestWithdrawal(body).ConfigureAwait(false);

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }

        public async Task<List<WithdrawalHistoryEntry>> GetWithdrawalHistory(WithdrawalHistoryContext context)
        {
            if (!context.Asset.ToRemoteCode(this).Equals(Asset.Btc.ToRemoteCode(this)))
                throw new ApiResponseException($"Exchange does not support {context.Asset.ShortCode} currency", this);

            var api = ApiProvider.GetApi(context);
            var remoteCode = context.Asset.ToRemoteCode(this);
            var r = await api.GetWalletHistory(remoteCode).ConfigureAwait(false);

            var history = new List<WithdrawalHistoryEntry>();

            foreach (var rHistory in r.Where(x => x.transactType.Equals("Withdrawal")))
            {
                history.Add(new WithdrawalHistoryEntry()
                {
                    Price = new Money(rHistory.amount * ConversionRate, context.Asset),
                    Fee = new Money(rHistory.fee * ConversionRate ?? 0.0m, context.Asset),
                    CreatedTimeUtc = rHistory.timestamp,
                    Address = rHistory.address,
                    WithdrawalRemoteId = rHistory.transactID,
                    WithdrawalStatus = ParseWithdrawalStatus(rHistory.transactStatus)
                });
            }

            return history;
        }

        private WithdrawalStatus ParseWithdrawalStatus(string statusRaw)
        {
            switch (statusRaw)
            {
                case "Canceled":
                    return WithdrawalStatus.Canceled;
                case "Completed":
                    return WithdrawalStatus.Completed;
                case "Confirmed":
                    return WithdrawalStatus.Confirmed;
                case "Pending":
                    return WithdrawalStatus.Awaiting;
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<WithdrawalCancelationResult> CancelWithdrawal(WithdrawalCancelationContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "token", context.WithdrawalRemoteId }
            };

            var r = await api.CancelWithdrawal(body);

            return new WithdrawalCancelationResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }

        public async Task<WithdrawalConfirmationResult> ConfirmWithdrawal(WithdrawalConfirmationContext context)
        {
            var api = ApiProvider.GetApi(context);

            var body = new Dictionary<string, object>
            {
                { "token", context.WithdrawalRemoteId }
            };

            var r = await api.ConfirmWithdrawal(body).ConfigureAwait(false);

            return new WithdrawalConfirmationResult()
            {
                WithdrawalRemoteId = r.transactID
            };
        }

        public async Task<VolumeResult> GetVolumeAsync(VolumeContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetLatestPriceAsync(context.Pair.Asset1.ToRemoteCode(this)).ConfigureAwait(false);

            var rPrice = r.FirstOrDefault();

            if (rPrice == null || rPrice.lastPrice.HasValue == false)
                throw new ApiResponseException($"Specified currency pair {context.Pair} is not supported by provider", this);

            return new VolumeResult()
            {
                Pair = context.Pair,
                Volume = rPrice.volume24h,
                Period = VolumePeriod.Day
            };
        }
    }
}
