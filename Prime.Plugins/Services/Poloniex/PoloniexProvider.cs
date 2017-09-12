﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using LiteDB;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using Prime.Utility;

namespace plugins
{
    public class PoloniexProvider : IExchangeProvider, IWalletService, IOhlcProvider
    {
        public Network Network { get; } = new Network("Poloniex");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:poloniex".GetObjectIdHashCode();

        public ObjectId Id => IdHash;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return new PoloniexClient() as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            return new PoloniexClient(key.Key, key.Secret) as T;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public Task<string> TestApi(ApiTestContext context)
        {
            var t = new Task<string>(delegate
            {
                try
                {
                    var api = GetApi<PoloniexClient>(context);
                    AsyncContext.Run(api.Wallet.GetBalancesAsync);
                    return null;
                }
                catch
                {
                    return "BAD";
                }
            });
            return t;
        }

        public Task<Money> GetLastPrice(PublicPriceContext context)
        {
            var t = new Task<Money>(() => Money.Zero);
            t.RunSynchronously();
            return t;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() =>
            {
                var api = this.GetApi<PoloniexClient>(context);
                var da = api.Markets.GetSummaryAsync();
                da.Wait();
                var aps = new AssetPairs();
                foreach (var assetPair in da.Result)
                {
                    var pair = assetPair.Key;
                    aps.Add(new AssetPair(pair.BaseCurrency, pair.QuoteCurrency, this));
                }

                return aps;
            });

            t.RunSynchronously();
            return t;
        }

        public bool CanMultiDepositAddress { get; }
        public bool CanGenerateDepositAddress { get; }

        public WalletAddresses FetchAllDepositAddresses(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public BalanceResults GetBalance(NetworkProviderPrivateContext context)
        {
            return context.L.Trace("Getting balance from " + Title, () =>
            {
                var api = this.GetApi<PoloniexClient>(context);
                var dat = api.Wallet.GetBalancesAsync();
                dat.Wait();
                var r = dat.Result;
                var results = new BalanceResults(this);
                foreach (var balance in r)
                {
                    var c = balance.Key.ToAsset(this);
                    results.AddBalance(c, (decimal) balance.Value.QuoteAvailable);
                    results.AddAvailable(c, (decimal) balance.Value.QuoteAvailable);
                    results.AddReserved(c, (decimal) balance.Value.QuoteOnOrders);
                }
                return results;
            });
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public WalletAddresses FetchDepositAddresses(WalletAddressAssetContext context)
        {
            var api = GetApi<PoloniexClient>(context);
            var dat = api.Wallet.GetDepositAddressesAsync();
            dat.Wait();
            var addresses = new WalletAddresses();
            foreach (var r in dat.Result.Where(x=>Equals(x.Key.ToAsset(this), context.Asset)))
            {
                if (string.IsNullOrWhiteSpace(r.Value))
                    continue;
                addresses.Add(new WalletAddress(this, r.Key.ToAsset(this)) { Address = r.Value});
            }
            return addresses;
        }

        public OhclData GetOhlc(OhlcContext context)
        {
            var pair = context.Pair;
            var market = context.Market;

            var api = GetApi<PoloniexClient>(context);
            var cpair = new CurrencyPair(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));
            var mp = MarketPeriod.Hours2;
            var ds = DateTime.UtcNow.AddDays(-10);
            var de = DateTime.UtcNow;
            var apir = AsyncContext.Run(()=> api.Markets.GetChartDataAsync(cpair, mp, ds, de));
            var r = new OhclData(market);
            var seriesid = OhlcResolutionAdapter.GetHash(pair, market, Network);
            foreach (var i in apir)
            {
                r.Add(new OhclEntry(seriesid, i.Time, this)
                {
                    Open = i.Open,
                    Close = i.Close,
                    Low = i.Low,
                    High = i.High,
                    VolumeTo = (long)i.VolumeQuote,
                    VolumeFrom = (long)i.VolumeBase,
                    WeightedAverage = i.WeightedAverage
                });
            }
            return r;
        }
    }
}