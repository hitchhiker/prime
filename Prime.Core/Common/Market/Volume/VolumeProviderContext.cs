﻿using System;
using Prime.Core;

namespace Prime.Core.Market
{
    public class VolumeProviderContext
    {
        public Action<PublicVolumeResponse> AfterData { get; set; }    

        public bool? UseReturnAll { get; set; }
        
        public bool OnlyBulk { get; set; }

        public VolumeProviderContext Clone()
        {
            return new VolumeProviderContext()
            {
                AfterData = AfterData,
                UseReturnAll = UseReturnAll,
                OnlyBulk = OnlyBulk
            };
        }
    }
}