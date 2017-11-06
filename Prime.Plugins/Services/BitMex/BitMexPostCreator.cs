using System;
using System.Collections.Generic;
using System.Text;
using Prime.Common;

namespace Prime.Plugins.Services.BitMex
{
    internal class BitMexPostCreator
    {
        private IDescribesAssets _provider;

        public BitMexPostCreator(IDescribesAssets provider)
        {
            _provider = provider;
        }

    }
}
