using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prime.Common
{
    public class NoAssetPairException : ApiResponseException
    {
        public NoAssetPairException(AssetPair pair, INetworkProvider provider, [CallerMemberName] string method = "Unknown") 
            : base($"Specified currency pair {pair} is not supported by provider", provider, method)
        {
            
        }

        public NoAssetPairException(Exception exception, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base(exception, provider, method)
        {
        }

        public NoAssetPairException(string message) : base(message)
        {
        }

        public NoAssetPairException(string message, INetworkProvider provider, [CallerMemberName] string method = "Unknown") : base(message, provider, method)
        {
        }
    }
}
