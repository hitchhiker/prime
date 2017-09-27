using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Plugins.Services.Kraken
{
    public class KrakenAuthenticator : BaseAuthenticator
    {
        public KrakenAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.AbsolutePath;
            var postData = request.Content.ReadAsStringAsync().Result;

            var nonce = ExtractNonce(postData);

            var headers = request.Headers;

            byte[] base64DecodedSecred = FromBase64(ApiKey.Secret);

            var np = nonce + Convert.ToChar(0) + postData;

            var pathBytes = FromUtf8(path);
            var hash256Bytes = HashSHA256Raw(np);

            var z = new byte[pathBytes.Length + hash256Bytes.Length];
            pathBytes.CopyTo(z, 0);
            hash256Bytes.CopyTo(z, pathBytes.Length);

            var signature = HashHMACSHA512(z, base64DecodedSecred);

            headers.Add("API-Key", ApiKey.Key);
            headers.Add("API-Sign", signature);
        }

        private long ExtractNonce(string postData)
        {
            var result = "";
            var nonce = postData.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(s => s.Contains("nonce"));

            if (nonce != null)
            {
                result = nonce.Substring(nonce.IndexOf("=", StringComparison.Ordinal) + 1);
            }

            return Convert.ToInt64(result);
        } 
    }
}
