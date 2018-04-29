using Prime.Core;

namespace Prime.Finance.Services.Services.Tidex
{
    internal class TidexAuthenticator : AuthenticatorHmacSha512Basic
    {
        public TidexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
