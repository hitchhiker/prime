using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.HitBtc
{
    public class HitBtcAuthenticator : BaseAuthenticator
    {
        public HitBtcAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var headers = request.Headers;
            //var nonce = GetLongNonce();
            //var postData = request.Content != null ? request.Content.ReadAsStringAsync().Result : "";

            //var properties = new string[]
            //{
            //    $"nonce={nonce}",
            //    $"apikey={ApiKey.Key}"
            //};

            //var oldQuery = String.IsNullOrEmpty(request.RequestUri.Query) ? "?" : request.RequestUri.Query;

            //var uri = request.RequestUri.AbsolutePath + oldQuery + string.Join("&", properties);

            //var message = uri + postData;

            //var signature = HashHMACSHA512Hex(message, ApiKey.Secret).ToLower();
            //headers.Add("X-Signature", signature);

            //request.RequestUri = new Uri(request.RequestUri, uri);

            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(FromUtf8($"{ApiKey.Key}:{ApiKey.Secret}"))}");

            //request.Content = new StringContent($"{ApiKey.Key}:{ApiKey.Secret}");
        }
    }
}
