using System.Net.Http;
using System.Threading;
using Prime.Common;

namespace Prime.Plugins.Services.Whaleclub
{
    public class WhaleclubAuthenticator : BaseAuthenticator
    {
        public WhaleclubAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (ApiKey == null)
                throw new ApiResponseException("This API cannot be used without authentication.");

            request.Headers.Add("Authorization", $"Bearer {ApiKey.Key}");
        }
    }
}