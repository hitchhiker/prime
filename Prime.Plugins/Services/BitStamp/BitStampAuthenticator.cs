using System;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Plugins.Services.BitStamp
{
    public class BitStampAuthenticator : BaseAuthenticator
    {
        public BitStampAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
