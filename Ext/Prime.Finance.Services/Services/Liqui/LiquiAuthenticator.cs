using Prime.Core;

namespace Prime.Finance.Services.Services.Liqui
{
    public class LiquiAuthenticator : AuthenticatorHmacSha512Basic
    {
        public LiquiAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
