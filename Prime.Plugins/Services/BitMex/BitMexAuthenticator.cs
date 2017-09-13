using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Plugins.Services.BitMex
{
    public class BitMexAuthenticator : BaseAuthenticator
    {
        #region Secrets

        // TODO: remove!
        [Obsolete]
        public const String ApiKey = "";
        [Obsolete]
        public const String ApiSecret = "";

        #endregion

        public BitMexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.PathAndQuery;

            var headers = request.Headers;

            string nonce = GetNonce().ToString();
            string message = $"GET{path}{nonce}";
            string signature = HashHMACSHA256Hex(message, ApiSecret);

            headers.Add("api-key", ApiKey);
            headers.Add("api-signature", signature);
            headers.Add("api-nonce", nonce);
        }

        private long GetNonce()
        {
            DateTime yearBegin = new DateTime(1990, 1, 1);
            return DateTime.UtcNow.Ticks - yearBegin.Ticks;
        }
    }
}
