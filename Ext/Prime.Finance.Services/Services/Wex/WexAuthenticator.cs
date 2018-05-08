using Prime.Core;

namespace Prime.Finance.Services.Services.Wex
{
    class WexAuthenticator : AuthenticatorHmacSha512Basic
    {
        public WexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
