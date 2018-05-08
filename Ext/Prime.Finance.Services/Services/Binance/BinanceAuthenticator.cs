using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Binance
{
    public class BinanceAuthenticator : BaseAuthenticator
    {

        public BinanceAuthenticator(ApiKey apiKey) : base(apiKey) { }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var properties = new string[]
            {
                $"timestamp={timeStamp}"
            };

            var initQueryString = String.IsNullOrWhiteSpace(request.RequestUri.Query) 
                ? ""
                : request.RequestUri.Query;

            var queryString = properties.Aggregate(initQueryString, (s, cur) => s += "&" + cur).TrimStart("?");

            var signature = HashHMACSHA256Hex(queryString , ApiKey.Secret);
            queryString = $"?{queryString}&signature={signature}";

            request.RequestUri = new Uri(request.RequestUri, queryString);

            headers.Add("X-MBX-APIKEY", ApiKey.Key);
        }
    }
}
