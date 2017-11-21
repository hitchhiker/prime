using System;
using System.Runtime.CompilerServices;

namespace Prime.Common
{
    public struct SuperResponse<T>
    {
        public SuperResponse(Exception e)
        {
            Response = default;
            FailReason = e.Message;
            Success = false;
            WasAttempted = true;
        }
        
        public SuperResponse(string failReason)
        {
            Response = default(T);
            FailReason = failReason;
            Success = false;
            WasAttempted = true;
        }

        public SuperResponse(T response)
        {
            Response = response;
            Success = true;
            FailReason = null;
            WasAttempted = true;
        }

        public readonly T Response;

        public readonly bool Success;

        public readonly bool WasAttempted;

        public string FailReason;

        public bool IsFailed => !Success;

        public bool IsNull => !Success || Response == null;
    }
}