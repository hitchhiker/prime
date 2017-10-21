using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class OhlcResolutionDataAdapterApi : IOhlcResolutionApi
    {
        public OhlcResolutionDataAdapterApi(OhlcResolutionAdapter adapter)
        {
            _adapter = adapter;
            Ctx = adapter.Ctx;
            L = new MessengerLogger(Ctx.Status);
        }

        private readonly OhlcResolutionAdapter _adapter;

        public readonly OhlcResolutionAdapterContext Ctx;

        public readonly Utility.ILogger L;

        public OhlcResolutionAdapter Adapter => _adapter;

        public OhlcData GetRange(TimeRange timeRange)
        {
            var r = GetRangeInternal(timeRange);
            if (r.IsEmpty())
                return r;

            if (timeRange.IsFromInfinity)
                r.Trim();

            r.DeGap();

            return r;
        }

        private OhlcData GetRangeInternal(TimeRange timeRange)
        {
            if (Ctx.PrimaryApiProvider == null)
                return null;

            if (Ctx.CurrencyConversionApiProvider != null)
            {
                var cr = Convert(timeRange);
                Ctx.Status(cr==null ? "Data missing" : "Received data");
                return cr;
            }

            Ctx.Status("Requesting @" + Ctx.PrimaryApiProvider.Title);
            var r = ApiCoordinator.GetOhlc(Ctx.PrimaryApiProvider, new OhlcContext(Ctx.Pair, Ctx.TimeResolution, timeRange, L));
            if (r.IsNull)
            {
                Ctx.Status("Data missing");
                return null;
            }
            
            r.Response.ForEach(x => x.CollectedNearLive = x.DateTimeUtc.IsLive(Ctx.TimeResolution));
            Ctx.Status("Received data");
            return r.Response;
        }

        private OhlcData Convert(TimeRange range)
        {
            Ctx.Status("Converting @" + Ctx.PrimaryApiProvider.Title + " " + Ctx.CurrencyConversionApiProvider.Title + " [1]");

            var pc = new OhlcContext(new AssetPair(Ctx.Pair.Asset1, Ctx.AssetIntermediary), Ctx.TimeResolution, range, L);
            var r1 = ApiCoordinator.GetOhlc(Ctx.PrimaryApiProvider, pc);
            if (r1.IsNull)
                return null;

            var d1 = r1.Response;

            Ctx.Status("Converting @" + Ctx.PrimaryApiProvider.Title + " " + Ctx.CurrencyConversionApiProvider.Title + " [2]");

            var pc2 = new OhlcContext(new AssetPair(Ctx.AssetIntermediary, Ctx.Pair.Asset2), Ctx.TimeResolution, range, L);

            var r2 = ApiCoordinator.GetOhlc(Ctx.CurrencyConversionApiProvider, pc2);
            if (r2.IsNull)
                return null;

            var d2 = r2.Response;

            if (d1.IsEmpty() || d2.IsEmpty())
                return null;

            if (d1.Count != d2.Count)
                return null;

            var ohcldata = new OhlcData(_adapter.TimeResolution)
            {
                ConvertedFrom = Ctx.AssetIntermediary,
                Network = Ctx.PrimaryApiProvider.Network
            };

            var seriesid = OhlcResolutionAdapter.GetHash(Ctx.Pair, range.TimeResolution, ohcldata.Network);

            foreach (var i in d1)
            {
                var i2 = d2.FirstOrDefault(x => x.DateTimeUtc == i.DateTimeUtc);
                if (i2 == null)
                    return null;

                ohcldata.Add(new OhlcEntry(seriesid, i.DateTimeUtc, Ctx)
                {
                    Open = i.Open * i2.Open,
                    Close = i.Close * i2.Close,
                    High = i.High * i2.High,
                    Low = i.Low * i2.Low,
                    VolumeTo = 0,
                    VolumeFrom = i2.VolumeFrom,
                    WeightedAverage = 0
                });
            }

            return ohcldata;
        }
    }
}