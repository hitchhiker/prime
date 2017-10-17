using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Plugins.Services.Korbit
{
    internal class KorbitAuthenticator : BaseAuthenticator
    {
        public KorbitAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: implement.
        }
    }
}
