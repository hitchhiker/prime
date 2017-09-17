using System;

namespace Prime.Core
{
    public struct ApiResponse<T>
    {
        public ApiResponse(Exception e)
        {
            Response = default(T);
            FailReason = e.Message;
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