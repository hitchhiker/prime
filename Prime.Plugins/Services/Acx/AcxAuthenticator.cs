using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Acx
{
    public class AcxAuthenticator : BaseAuthenticator
    {

        public AcxAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
        
        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            }
    }
}
