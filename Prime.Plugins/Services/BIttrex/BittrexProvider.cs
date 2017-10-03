using System;
using System.Linq;
using System.Threading.Tasks;
using Bittrex;
using LiteDB;
using Newtonsoft.Json.Linq;
using Prime.Core;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.BitMex;
using Prime.Utility;
using RestEase;

namespace Prime.Plugins.Services.Bittrex
{
    public class BittrexProvider : IExchangeProvider, IWalletService, IApiProvider
    {
        private const string BittrexApiVersion = "v1.1";
        private const string BittrexApiUrl = "https://bittrex.com/api/" + BittrexApiVersion;

        private const string ErrorText_AddressIsGenerating = "ADDRESS_GENERATING";

        public Network Network { get; } = new Network("Bittrex");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        private static readonly ObjectId IdHash = "prime:bittrex".GetObjectIdHashCode();
        //3d3bdcb685a3455f965f0e78ead0cbba
        public ObjectId Id => IdHash;

        private static readonly NoRateLimits Limiter = new NoRateLimits();
        public IRateLimiter RateLimiter => Limiter;

        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<IBittrexApi>(BittrexApiUrl) as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            return RestClient.For<IBittrexApi>(BittrexApiUrl, new BittrexAuthenticator(key).GetRequestModifier) as T;
        }

        public async Task<bool> TestApiAsync(ApiTestContext context)
        {
            var api = GetApi<IBittrexApi>(context);
            var r = await api.GetAllBalances();

            return r != null && r.success && r.result != null;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public async Task<LatestPrice> GetLatestPriceAsync(PublicPriceContext context)
        {
            var api = GetApi<IBittrexApi>(context);
            var pairCode = context.Pair.TickerDash();
            var r = await api.GetTicker(pairCode);

            CheckResponseErrors(r);

            var latestPrice = new LatestPrice(new Money(r.result.Last, context.Pair.Asset1))
            {
                BaseAsset = context.Pair.Asset2
            };

            return latestPrice;
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
            var api = GetApi<IBittrexApi>(context);
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
            var api = GetApi<IBittrexApi>(context);

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

        public bool CanMultiDepositAddress => false;

        public bool CanGenerateDepositAddress => true;

        public async Task<WalletAddresses> GetAddressesAsync(WalletAddressContext context)
        {
            var api = GetApi<IBittrexApi>(context);

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

        public async Task<WalletAddresses> GetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var api = GetApi<IBittrexApi>(context);

            if (context.CanGenerateAddress)
            {
                var r = await api.GetDepositAddress(context.Asset.ToRemoteCode(this));
                var addresses = new WalletAddresses();

                if (r.success)
                {
                    addresses.Add(new WalletAddress(this, context.Asset)
                    {
                        Address = r.result.Address
                    });
                }
                else
                {
                    if (String.IsNullOrEmpty(r.message) == false && r.message.Contains(ErrorText_AddressIsGenerating) == false)
                    {
                        throw new ApiResponseException("Unexpected error during address generation", this);
                    }
                }

                return addresses;
            }
            else
            {
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
            }
        }

        public OhlcData GetOhlc(AssetPair pair, TimeResolution market)
        {
            return null;
        }

        private void CheckResponseErrors<T>(BittrexSchema.BaseResponse<T> response)
        {
            if (response.success == false)
                throw new ApiResponseException($"API error: {response.message}", this);
        }
    }
}