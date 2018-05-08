using System;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Bittrex
{
    internal class BittrexAuthenticator : BaseAuthenticator
    {
        public BittrexAuthenticator(ApiKey apiKey) : base(apiKey) { }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;

            var nonce = GetLongNonce().ToString();

            var properties = new string[]
            {
                $"apiKey={ApiKey.Key}",
                $"nonce={nonce}"
            };

            var aggrInitialValue = string.IsNullOrEmpty(request.RequestUri.Query)
                ? "?"
                : request.RequestUri.Query + "&";

            var queryParams = aggrInitialValue + string.Join("&", properties);
            request.RequestUri = new Uri(request.RequestUri, queryParams);

            var sign = HashHMACSHA512Hex(request.RequestUri.OriginalString, ApiKey.Secret);

            headers.Add("apisign", sign);
        }
    }
}
