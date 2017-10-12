using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;
using RestEase;
using Prime.Utility;

namespace plugins
{
    public class CoinbaseAuthenticator : BaseAuthenticator
    {
        public CoinbaseAuthenticator(ApiKey apiKey) : base(apiKey) { }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = ApiKey.Key;
            var secret = ApiKey.Secret;

            var path = request.RequestUri.PathAndQuery;
            var ts = Math.Round(DateTime.UtcNow.ToUnixTimeStampSimple(), 0).ToString(CultureInfo.InvariantCulture);

            var headers = request.Headers;

            headers.Add("CB-ACCESS-KEY", key);
            headers.Add("CB-ACCESS-TIMESTAMP", ts);
            headers.Add("CB-VERSION", "2017-03-24");
            headers.Add("CB-ACCESS-SIGN", HashHMACSHA256Hex(ts + request.Method.ToString().ToUpper() + path, secret));
        }
    }
}