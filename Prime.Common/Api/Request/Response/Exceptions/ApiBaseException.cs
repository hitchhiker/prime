using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prime.Common
{
    public class ApiBaseException : Exception
    {
        public ApiBaseException(Exception exception, INetworkProvider provider, [CallerMemberName] string method = "Unknown")
        {
            Message = exception.Message + " - " + method + " in " + provider.Title + " provider.";
        }

        public ApiBaseException(string message)
        {
            Message = message;
        }

        public ApiBaseException(string message, INetworkProvider provider, [CallerMemberName] string method = "Unknown")
        {
            Message = message + " - " + method + " in " + provider.Title + " provider.";
        }

        public override string Message { get; }
    }
}
