using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Exx
{
    public class ExxAuthenticator : BaseAuthenticator
    {
        //TODO - SC - Add to keymanager once it is working again
        private const string publicKey = "c66c328c-8752-4c74-9d9d-099a97e3b520";
        private const string privateKey = "7a50b3c7c18a21d909c82303fd18db73d1858ef5";

        public ExxAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
          
        }
    }
}
