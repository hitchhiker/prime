using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Prime.Core;

namespace Prime.Finance
{
    public class NoTradeOrderException : ApiResponseException
    {
        public NoTradeOrderException(RemoteIdContext context, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base($"Order \"{context.RemoteGroupId}\" does not exist", provider, method)
        {

        }
    }
}
