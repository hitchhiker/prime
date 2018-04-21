using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.HitBtc
{
    public class HitBtcAuthenticator : BaseAuthenticator
    {
        public HitBtcAuthenticator(ApiKey apiKey) : base(apiKey) { }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(FromUtf8($"{ApiKey.Key}:{ApiKey.Secret}"))}");
        }
    }
}
