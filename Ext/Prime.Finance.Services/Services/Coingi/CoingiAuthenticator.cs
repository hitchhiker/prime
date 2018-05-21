using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Coingi
{
    public class CoingiAuthenticator : BaseAuthenticator
    {
        //TODO - SC - Replace public and private keys
        private const string publicKey = "tzwUJmMBAACrPMIWDPf3d/1qUTN0fVn/fqTTWzSn332LW3hQuFZsI/oCPkQ83rU+";
        private const string secret = "UJ8ZfuuHFdpydXnyKSaAudwfo00TdQD22K28XmRK";

        public CoingiAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = request.Content?.ReadAsStringAsync()?.Result;

            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var signature = HashHMACSHA256Hex($"{nonce}${ /*ApiKey.Key*/publicKey}", /*ApiKey.Secret*/secret).ToLower();

            string strPostParams;

            if (string.IsNullOrWhiteSpace(content))
            {
                strPostParams = "{\"token\":\"" + /*ApiKey.Key*/publicKey + "\",\"signature\":\"" + signature + "\", \"nonce\":\"" + nonce + "\"}";
            }
            else
            {
                content = content?.Replace("}", "").Replace("{", "");
                strPostParams = "{\"token\":\"" + /*ApiKey.Key*/publicKey + "\",\"signature\":\"" + signature + "\", \"nonce\":\"" + nonce + "\", " + content + "}";
            }

            var contentBuilder = new StringBuilder(strPostParams);
            request.Content = new StringContent(contentBuilder.ToString(), Encoding.UTF8, "application/json");
        }
    }
}
