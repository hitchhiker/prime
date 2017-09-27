using System;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Plugins.Services.Poloniex
{
    public class PoloniexAuthenticator : BaseAuthenticator
    {
        #region Secrets

        public const String Key = "TRPQNJYF-MMZJ6U8H-BEKIN2ZO-RXKYEH9Y";
        public const String Secret = "4d286c51c88d4dcc33c5a4e7cb871fd9d8a141002e96b3d72bab5aa9aad52b3eb1baff59fba0fcce169607993227884bd8f46115115b4d7fb77f445ac5dfe0a8";

        #endregion

        public PoloniexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var postData = request.Content.ReadAsStringAsync().Result;
            var sign = HashHMACSHA512Hex(postData, Secret);

            headers.Add("Key", Key);
            headers.Add("Sign", sign);
        }
    }
}
