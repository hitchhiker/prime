using System.Net.Http;
using System.Threading;
using Prime.Common;

namespace Prime.Finance.Services.Services.Yobit
{
    public class YobitAuthenticator : BaseAuthenticator
    {

        public YobitAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var strForSign = request.Content?.ReadAsStringAsync()?.Result;
            
            var signature = HashHMACSHA512Hex(strForSign, ApiKey.Secret);

            headers.Add("Key", ApiKey.Key);
            headers.Add("Sign", signature);
        }
    }
}
