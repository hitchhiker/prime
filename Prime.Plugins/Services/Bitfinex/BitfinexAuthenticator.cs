using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Prime.Common;

namespace Prime.Plugins.Services.Bitfinex
{
    internal class BitfinexAuthenticator : BaseAuthenticator
    {
        public BitfinexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var bodyPrevJson = request.Content.ReadAsStringAsync().Result;
            var bodyPrev = JsonConvert.DeserializeObject<BitfinexSchema.BaseRequest>(bodyPrevJson);
            var nonce = GetUnixEpochNonce();

            bodyPrev.request = request.RequestUri.AbsolutePath;
            bodyPrev.nonce = nonce.ToString();

            var bodyJson = JsonConvert.SerializeObject(bodyPrev);
            var payload = ToBase64(bodyJson);
            var signature = HashHMACSHA384Hex(payload, ApiKey.Secret);

            request.Content = new StringContent(bodyJson);

            request.Headers.Add("X-BFX-APIKEY", ApiKey.Key);
            request.Headers.Add("X-BFX-PAYLOAD", payload);
            request.Headers.Add("X-BFX-SIGNATURE", signature);
        }
    }
}
