using System;
using System.Net.Http;
using System.Threading;
using Prime.Common;

namespace Prime.Finance.Services.Services.Bitsane
{
    public class BitsaneAuthenticator : BaseAuthenticator
    {

        public BitsaneAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var headers = request.Headers;

            var strParams = request.Content?.ReadAsStringAsync()?.Result;

            string payload = "{\"nonce\":\"" + nonce + "\"}";

            if (!string.IsNullOrWhiteSpace(strParams))
            {
                payload = $"{payload.Replace("}","")},{strParams.Replace("{","")}";
            }

            payload = ToBase64(payload);

            request.Content = new StringContent(payload);

            headers.Add("X-BS-APIKEY", ApiKey.Key);
            headers.Add("X-BS-PAYLOAD", payload);
            headers.Add("X-BS-SIGNATURE", HashHMACSHA384Hex(payload, ApiKey.Secret));
        }
    }
}
