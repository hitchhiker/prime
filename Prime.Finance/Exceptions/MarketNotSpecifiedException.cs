using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Prime.Core;

namespace Prime.Finance
{
    public class MarketNotSpecifiedException : ApiBaseException
    {
        public MarketNotSpecifiedException(INetworkProvider provider, [CallerMemberName] string method = "Unknown")
            : base($"Market is required but not specified", provider, method)
        {

        }
    }
}
