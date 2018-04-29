using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.BitFlyer
{
    internal class BitFlyerAuthenticator : BaseAuthenticator
    {
        public BitFlyerAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
        }
    }
}
