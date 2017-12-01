using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Whaleclub
{
    public class WhaleclubProvider : IAssetPairsProvider
    {
        private const string WhaleclubApiVersion = "v1";
        private const string WhaleclubApiUrl = "https://api.whaleclub.co/" + WhaleclubApiVersion;
        
        private static readonly ObjectId IdHash = "prime:whaleclub".GetObjectIdHashCode();

        public ObjectId Id => IdHash;
        public Network Network { get; } = Networks.I.Get("Whaleclub");
        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        
        public char? CommonPairSeparator => '-';
        
        private RestApiClientProvider<IWhaleclubApi> ApiProvider { get; }
        
        // By default, each API token is rate limited at 60 requests per minute.
        // Additionally, requests are throttled up to a maximum of 20 requests per second.
        private static IRateLimiter Limiter = new PerSecondRateLimiter(1, 1);
        public IRateLimiter RateLimiter => Limiter;
        public bool IsDirect => true;

        public WhaleclubProvider()
        {
            ApiProvider = new RestApiClientProvider<IWhaleclubApi>(WhaleclubApiUrl, this, k => new WhaleclubAuthenticator(k).GetRequestModifier);
        }
        
        public Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            throw new System.NotImplementedException();
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);
            var r = await api.GetMarkets().ConfigureAwait(false);
            
            var pairs = new AssetPairs();
            foreach (var pair in r)
            {
                pairs.Add(pair.Key.ToAssetPair(this));
            }

            return pairs;
        }
    }
}
