using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using plugins;
using Prime.Core;

namespace Prime.Plugins.Services.Base
{
    internal interface IApiProvider
    {
        T GetApi<T>(NetworkProviderContext context) where T : class;

        T GetApi<T>(NetworkProviderPrivateContext context) where T : class;
    }
}
