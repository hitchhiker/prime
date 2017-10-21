using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class PageUris
    {
        private PageUris() {}

        public static PageUris I => Lazy.Value;
        private static readonly Lazy<PageUris> Lazy = new Lazy<PageUris>(()=>new PageUris());

        private IReadOnlyList<IPageUriProvider> _providers;
        public IReadOnlyList<IPageUriProvider> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<IPageUriProvider>().Where(x => !x.Disabled).ToList());
    }
}