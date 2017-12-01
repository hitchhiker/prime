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
            request.Headers.Add("Authorization", $"Bearer {ApiKey.Key}");
        }
    }
}