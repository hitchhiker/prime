using System;
using System.Collections.Generic;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services.Wex
{
    class WexAuthenticator : AuthenticatorHmacSha512Basic
    {
        public WexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
