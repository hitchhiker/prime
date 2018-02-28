using System;
using System.Runtime.CompilerServices;

namespace Prime.Common
{
    public class ApiResponseException : ApiBaseException
    {
        public ApiResponseException(Exception exception, INetworkProvider provider, [CallerMemberName]string method = "Unknown") : base(exception, provider, method)
        {
        }

        public ApiResponseException(string message) : base(message)
        {
        }

        public ApiResponseException(string message, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base(message, provider, method)
        {
        }
    }
}