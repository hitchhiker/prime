using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Core;

namespace Prime.Finance
{
    public static class UserContextExtensionMethods
    {
        public static FinanceUserContext Finance(this UserContext context)
        {
            return context.GetInstance<FinanceUserContext>(() => new FinanceUserContext(context));
        }
    }
}