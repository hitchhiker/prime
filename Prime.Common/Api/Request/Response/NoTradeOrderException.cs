using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prime.Common.Api.Request.Response
{
    public class NoTradeOrderException : ApiResponseException
    {
        public NoTradeOrderException(RemoteIdContext context, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base($"Order \"{context.RemoteId}\" does not exist", provider, method)
        {

        }

        public NoTradeOrderException(Exception exception, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base(exception, provider, method)
        {
        }

        public NoTradeOrderException(string message) : base(message)
        {
        }

        public NoTradeOrderException(string message, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base(message, provider, method)
        {
        }
    }
}
