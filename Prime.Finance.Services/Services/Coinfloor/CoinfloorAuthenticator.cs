using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Coinfloor
{
    public class CoinfloorAuthenticator : BaseAuthenticator
    {

        public CoinfloorAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var credentials = $"{ApiKey.Key}/{ApiKey.Secret}:{ApiKey.Extra}";
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", ToBase64(credentials));
        }
    }
}
