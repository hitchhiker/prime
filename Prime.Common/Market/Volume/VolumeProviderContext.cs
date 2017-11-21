using System;
using Prime.Common;

namespace Prime.Core.Market
{
    public class VolumeProviderContext
    {
        public Action<PublicVolumeResponse> AfterData { get; set; }    

        public bool? UseReturnAll { get; set; }
        
        public bool OnlyBulk { get; set; }
    }
}