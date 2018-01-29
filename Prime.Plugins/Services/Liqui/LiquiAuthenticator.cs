using System;
using System.Collections.Generic;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services.Liqui
{
    public class LiquiAuthenticator : AuthenticatorHmacSha512Basic
    {
        public LiquiAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }
    }
}
