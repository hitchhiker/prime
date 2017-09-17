using Prime.Utility;

namespace Prime.Core
{
    public class ApiTestContext : NetworkProviderPrivateContext
    {
        public readonly ApiKey ApiTestKey;

        public ApiTestContext(ApiKey apiTestKey, ILogger logger = null) : base(null, logger)
        {
            ApiTestKey = apiTestKey;
        }

        public override ApiKey GetKey(INetworkProvider provider)
        {
            return ApiTestKey;
        }

        public override bool IsPublic => false;
    }
}