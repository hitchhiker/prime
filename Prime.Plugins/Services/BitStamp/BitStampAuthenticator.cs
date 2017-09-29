using System;
using System.Net.Http;
using System.Threading;
using Prime.Core;
using Prime.Utility;

namespace Prime.Plugins.Services.BitStamp
{
    public class BitStampAuthenticator : BaseAuthenticator
    {
        public BitStampAuthenticator(ApiKey apiKey) : base(apiKey)
        {

        }


        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var nonce = GetLongNonce().ToString();
            var customerId = ApiKey.Extra;

            var message = nonce + customerId + ApiKey.Secret;

            var signature = HashHMACSHA256Hex(message, ApiKey.Secret);

            headers.Add("key", ApiKey.Key); // ApiKey.Key
            headers.Add("nonce", nonce);
            headers.Add("signature", signature);
        }
    }
}
