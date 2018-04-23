using Prime.Common;

namespace Prime.Finance.Services.Services.Base
{
    internal interface IApiProvider
    {
        T GetApi<T>(NetworkProviderContext context) where T : class;

        T GetApi<T>(NetworkProviderPrivateContext context) where T : class;
    }
}
