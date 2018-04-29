using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public class OrderCancelationResponse : ResponseModelBase
    {
        public bool Success { get; }

        public OrderCancelationResponse(bool success)
        {
            Success = success;
        }
    }
}
