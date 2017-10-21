using System;

namespace Prime.Common
{
    public class ProviderDataCaches : AssociatedDatasBase<ProviderDataCacheBase>
    {
        private ProviderDataCaches() {}

        public static ProviderDataCaches I => Lazy.Value;
        private static readonly Lazy<ProviderDataCaches> Lazy = new Lazy<ProviderDataCaches>(()=>new ProviderDataCaches());


    }
}