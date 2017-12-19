using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class ContextArgumentException : ApiResponseException
    {
        public ContextArgumentException(Exception exception, INetworkProvider provider, string method = "Unknown") : base(exception, provider, method)
        {
        }

        public ContextArgumentException(string message) : base(message)
        {
        }

        public ContextArgumentException(string message, INetworkProvider provider, string method = "Unknown") : base(message, provider, method)
        {
        }
    }
}
