using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;

namespace Prime.Plugins.Services.Tidex
{
    internal class TidexAuthenticator : AuthenticatorHmacSha512Basic
    {
        public TidexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
