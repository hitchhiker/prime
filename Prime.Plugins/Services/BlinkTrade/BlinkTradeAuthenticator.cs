using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BlinkTrade
{
    public class BlinkTradeAuthenticator : BaseAuthenticator
    {

        public BlinkTradeAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
            
            var signature = HashHMACSHA256(nonce.ToString(), ApiKey.Secret);

            request.Headers.Add("APIKey", ApiKey.Key);
            request.Headers.Add("Nonce", nonce.ToString());
            request.Headers.Add("Signature", signature);
        }
    }
}
