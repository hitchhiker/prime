﻿using Prime.Core;

namespace Prime.Core
{
    public interface IOhlcResolutionAdapterStorage : IOhlcResolutionAdapter
    {
        void StoreRange(OhlcData data, TimeRange rangeAttempted);

        CoverageMapBase CoverageMap { get; }
    }
}