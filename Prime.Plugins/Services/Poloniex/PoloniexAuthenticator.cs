using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Plugins.Services.Poloniex
{
    public class PoloniexAuthenticator : BaseAuthenticator
    {
        #region Secrets

        public const String Key = "2BNI94RA-5TMCCY9M-IM4HZN0P-BSASO5RG";
        public const String Secret = "884a493b59e7522c559be3c3d74303da3529d0f0e770034af674d7fb7781d8f635e2d48faf70b946cb9a67961268ef04f1de4c356f2a87829f669ca9f60926e5";

        #endregion

        public PoloniexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.PathAndQuery;

            var headers = request.Headers;

            var nonce = GetNonce().ToString();

            // TODO: remove KEYS!
            headers.Add("Key", Key);
            // headers.Add("Sign", HashHMACSHA512Hex("data", Secret));
        }
    }
}
