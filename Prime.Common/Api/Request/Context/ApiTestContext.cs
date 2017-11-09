using Prime.Utility;

namespace Prime.Common
{
    public class ApiPrivateTestContext : NetworkProviderPrivateContext
    {
        public readonly ApiKey ApiTestKey;

        public ApiPrivateTestContext(ApiKey apiTestKey, ILogger logger = null) : base(null, logger)
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