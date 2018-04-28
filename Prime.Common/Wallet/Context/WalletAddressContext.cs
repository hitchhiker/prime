﻿using Prime.Common;

namespace Prime.Common
{
    public class WalletAddressContext : NetworkProviderPrivateContext
    {
        public WalletAddressContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
            
        }
    }
}