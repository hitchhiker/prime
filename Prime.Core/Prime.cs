using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public class Prime
    {
        public readonly ExtensionManager Extensions;

        public Prime(ServerContext context)
        {
            Extensions = new ExtensionManager(context);
        }
    }
}
