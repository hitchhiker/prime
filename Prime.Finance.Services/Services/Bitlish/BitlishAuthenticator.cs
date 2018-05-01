using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Bitlish
{
    public class BitlishAuthenticator : BaseAuthenticator
    {

        public BitlishAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.AbsolutePath.Contains("signin"))
            {
                request.RequestUri =
                    new Uri(
                        $"{request.RequestUri.AbsoluteUri}?login={ApiKey.Extra}&passwd={ApiKey.Secret}&token={ApiKey.Key}");
            }
            else
            {
                var parameters = request.RequestUri.Query.Replace("?", "").Split('&');

                var authenticationTokenParameter =
                    parameters.FirstOrDefault(x => x.StartsWith("authentication_token="));

                if (authenticationTokenParameter == null) return;

                var token = authenticationTokenParameter.Substring(21); // 21 -> That is the string after 'authentication_token='.

                if (!string.IsNullOrWhiteSpace(token))
                    request.Headers.Add("token", token);
            }
        }
    }
}
