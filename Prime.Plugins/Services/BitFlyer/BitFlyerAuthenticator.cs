using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Plugins.Services.BitFlyer
{
    internal class BitFlyerAuthenticator : BaseAuthenticator
    {
        public BitFlyerAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: implement.
        }
    }
}
