using System;

namespace Prime.Core
{
    public class ApiResponseException : Exception
    {
        public ApiResponseException(string message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}