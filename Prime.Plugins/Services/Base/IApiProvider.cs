using Prime.Common;

namespace Prime.Plugins.Services.Base
{
    internal interface IApiProvider
    {
        T GetApi<T>(NetworkProviderContext context) where T : class;

        T GetApi<T>(NetworkProviderPrivateContext context) where T : class;
    }
}
