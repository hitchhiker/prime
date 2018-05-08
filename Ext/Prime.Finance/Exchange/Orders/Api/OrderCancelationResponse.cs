using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Finance
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
