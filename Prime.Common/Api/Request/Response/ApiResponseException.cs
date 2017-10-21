using System;
using System.Runtime.CompilerServices;

namespace Prime.Common
{
    public class ApiResponseException : Exception
    {
        public ApiResponseException(Exception exception, INetworkProvider provider, [CallerMemberName] string method = "Unknown")
        {
            Message = exception.Message + " - " + method + " in " + provider.Title + " provider.";
        }

        public ApiResponseException(string message)
        {
            Message = message;
        }

        public ApiResponseException(string message, INetworkProvider provider, [CallerMemberName] string method = "Unknown")
        {
            Message = message + " - " + method + " in " + provider.Title + " provider.";
        }

        public override string Message { get; }
    }
}