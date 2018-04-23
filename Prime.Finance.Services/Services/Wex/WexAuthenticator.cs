using Prime.Common;

namespace Prime.Finance.Services.Services.Wex
{
    class WexAuthenticator : AuthenticatorHmacSha512Basic
    {
        public WexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
