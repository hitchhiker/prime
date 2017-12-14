using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using Prime.Common;

namespace Prime.Plugins.Services.Cryptopia
{
    class CryptopiaAuthenticator : BaseAuthenticator
    {
        public CryptopiaAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = GetUnixEpochNonce();
            var uri = request.RequestUri.ToString();

            var postData = request.Content?.ReadAsStringAsync().Result;

            var hashedPostData =
                String.IsNullOrWhiteSpace(postData) ? "" : HashMD5(postData);

            var signature = $"{ApiKey.Key}POST{HttpUtility.UrlEncode(uri.ToLower())}{nonce}{hashedPostData}";
            var hmacSignature = ToBase64(HashHMACSHA256Raw(FromUtf8(signature), FromBase64(ApiKey.Secret)));
            var headerValue = $"amx {ApiKey.Key}:{hmacSignature}:{nonce}";

            request.Headers.Add("Authorization", headerValue);
        }
    }
}
