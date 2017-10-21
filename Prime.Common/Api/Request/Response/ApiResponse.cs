using System;
using System.Runtime.CompilerServices;

namespace Prime.Common
{
    public struct ApiResponse<T>
    {
        public ApiResponse(Exception e)
        {
            Response = default(T);
            FailReason = e.Message;
            Success = false;
        }

        public ApiResponse(string message, INetworkProvider provider, [CallerMemberName] string method = "Unknown")
        {
            Response = default(T);
            FailReason = message + " - " + method + " in " + provider.Title + " provider.";
            Success = false;
        }

        public ApiResponse(string failReason)
        {
            Response = default(T);
            FailReason = failReason;
            Success = false;
        }

        public ApiResponse(T response)
        {
            Response = response;
            Success = true;
            FailReason = null;
        }

        public readonly T Response;

        public readonly bool Success;

        public string FailReason;

        public bool IsFailed => !Success;

        public bool IsNull => !Success || Response == null;
    }
}