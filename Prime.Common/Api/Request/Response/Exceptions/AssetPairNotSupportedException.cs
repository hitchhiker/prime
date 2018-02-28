using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prime.Common
{
    public class AssetPairNotSupportedException : ApiResponseException
    {
        public AssetPairNotSupportedException(AssetPair pair, INetworkProvider provider, [CallerMemberName] string method = "Unknown") 
            : base($"Specified currency pair {pair} is not supported by provider", provider, method)
        {
            
        }
    }
}
