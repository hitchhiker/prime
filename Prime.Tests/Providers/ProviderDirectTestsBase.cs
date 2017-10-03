using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;

namespace Prime.Tests
{
    public abstract class ProviderDirectTestsBase
    {
        public INetworkProvider Provider { get; protected set; }

        public virtual async Task TestApisAsync()
        {
            var p = IsType<INetworkProviderPrivate>();
            if (p.Success)
                await TestApiAsync(p.Provider);
        }

        public virtual async Task TestGetDepositAddressesAsync()
        {
            var p = IsType<IDepositService>();
            if (p.Success)
                await GetDepositAddressesAsync(p.Provider);
        }

        public async Task TestApiAsync(INetworkProviderPrivate provider)
        {
            var ctx = new ApiTestContext(UserContext.Current.GetApiKey(provider));

            try
            {
                var r = await provider.TestApiAsync(ctx);
                Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        public async Task GetDepositAddressesAsync(IDepositService provider)
        {
            var asset = "BTC".ToAsset(provider);

            var ctx = new WalletAddressAssetContext(asset, false, UserContext.Current);

            try
            {
                var r = await provider.GetAddressesForAssetAsync(ctx);
                Assert.IsTrue(r != null);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /*
        public void TestPortfolioAccountBalances()
        {
            var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

            var c = new PortfolioProviderContext(UserContext.Current, provider, UserContext.Current.BaseAsset, 0);
            var scanner = new PortfolioProvider(c);
            try
            {
                scanner.Update();
                foreach (var i in scanner.Items)
                    Console.WriteLine(i.Asset.ShortCode + " " + i.AvailableBalance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        

            public async Task GetOhlcDataAsync()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

                var ohlcContext = new OhlcContext(new AssetPair("BTC", "USD"), TimeResolution.Minute, new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute), null);

                try
                {
                    var ohlc = await provider.GetOhlcAsync(ohlcContext);

                    foreach (var data in ohlc)
                    {
                        Console.WriteLine($"{data.DateTimeUtc}: {data.High} {data.Low} {data.Open} {data.Close}");
                    }

                    Console.WriteLine($"Entries count: {ohlc.Count}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public async Task GetLatestPriceAsync()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

                var ctx = new PublicPricesContext("BTC".ToAssetRaw(), new List<Asset>()
                {
                    "USD".ToAsset(provider)
                });

                try
                {
                    var c = await provider.GetLatestPricesAsync(ctx);

                    Console.WriteLine($"Base asset: {ctx.BaseAsset}\n");

                    foreach (Money price in c.Prices)
                    {
                        Console.WriteLine(price.Display);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public async Task GetAssetPairsAsync()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
                var ctx = new NetworkProviderContext();

                try
                {
                    var pairs = await provider.GetAssetPairs(ctx);

                    foreach (var pair in pairs)
                    {
                        Console.WriteLine($"{pair}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public async Task GetBalancesAsync()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
                var ctx = new NetworkProviderPrivateContext(UserContext.Current);

                try
                {
                    var balances = await provider.GetBalancesAsync(ctx);

                    foreach (var balance in balances)
                    {
                        Console.WriteLine($"{balance.Asset} : {balance.Balance}, {balance.Available}, {balance.Reserved}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            public async Task GetAllDepositAddressesAsync()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

                var ctx = new WalletAddressContext(false, UserContext.Current);

                try
                {
                    var addresses = await provider.GetAddressesAsync(ctx);

                    Console.WriteLine("All addresses:");

                    foreach (var address in addresses)
                    {
                        Console.WriteLine($"{address.Asset}: {address.Address}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }*/

        protected (bool Success, T Provider) IsType<T>()
        {
            if (!(Provider is T tp))
                Assert.Fail(Provider.Title + " is not of type " + nameof(INetworkProviderPrivate));
            else
                return (true, tp);
            return (false, default(T));
        }
    }
}