using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Korbit
{
    internal class KorbitAuthenticator : BaseAuthenticator
    {
        public KorbitAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
        }
    }
}
