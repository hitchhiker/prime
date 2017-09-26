using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Utility;

namespace plugins
{
    public class BitStampAuthenticator : BaseAuthenticator
    {
        public BitStampAuthenticator(ApiKey apiKey) : base(apiKey)
        {

        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var nonce = GetNonce().ToString();
            var customerId = ApiKey.Extra;

            var message = nonce + customerId + ApiKey.Secret;

            var signature = HashHMACSHA256Hex(message, ApiKey.Secret);

            headers.Add("key", ApiKey.Key); // ApiKey.Key
            headers.Add("nonce", nonce);
            headers.Add("signature", signature);
        }
    }
}
