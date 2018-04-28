using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Prime.Common;

namespace Prime.Finance.Services.Services.Dsx
{
    public class DsxAuthenticator : BaseAuthenticator
    {

        public DsxAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            parameters = parameters?.Replace(":", "=").Replace(",", "&").Replace("{", "").Replace("}", "").Replace("\"", "");
            var signature = HashHMACSHA512(parameters, ApiKey.Secret);

            headers.Add("Key", ApiKey.Key);
            headers.Add("Sign", signature);

            request.Content = new StringContent(parameters, Encoding.UTF8, "application/x-www-form-urlencoded");

            if (request.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }
    }
}
