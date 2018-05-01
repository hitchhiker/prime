using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public class ContextArgumentException : ApiResponseException
    {
        public ContextArgumentException(string message, INetworkProvider provider, string method = "Unknown") : base(message, provider, method)
        {
        }
    }
}
