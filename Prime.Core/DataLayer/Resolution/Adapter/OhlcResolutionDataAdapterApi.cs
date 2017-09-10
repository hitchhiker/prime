using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class OhlcResolutionDataAdapterApi : IOhclResolutionApi
    {
        public OhlcResolutionDataAdapterApi(OhlcResolutionDataAdapter adapter)
        {
            _adapter = adapter;
            Ctx = adapter.Ctx;
            L = new Utility.Logger(Ctx.Status);
        }

        private readonly OhlcResolutionDataAdapter _adapter;

        public readonly OhlcResolutionAdapterContext Ctx;

        public readonly Utility.Logger L;

        public OhlcResolutionDataAdapter Adapter => _adapter;

        public OhclData GetRange(TimeRange timeRange)
        {
            var r = GetRangeInternal(timeRange);
            if (r.IsEmpty())
                return r;

            if (timeRange.IsFromInfinity)
                r.Trim();

            r.DeGap();

            return r;
        }

        private OhclData GetRangeInternal(TimeRange timeRange)
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
            var r = Ctx.PrimaryApiProvider.GetOhlc(new OhlcContext(Ctx.Pair, Ctx.TimeResolution, timeRange, L));
            Ctx.Status(r == null ? "Data missing" : "Received data");
            return r;
        }

        private OhclData Convert(TimeRange range)
        {
            Ctx.Status("Converting @" + Ctx.PrimaryApiProvider.Title + " " + Ctx.CurrencyConversionApiProvider.Title + " [1]");

            var pc = new OhlcContext(new AssetPair(Ctx.Pair.Asset1, Ctx.AssetIntermediary), Ctx.TimeResolution, range, L);
            var d1 = Ctx.PrimaryApiProvider.GetOhlc(pc);

            Ctx.Status("Converting @" + Ctx.PrimaryApiProvider.Title + " " + Ctx.CurrencyConversionApiProvider.Title + " [2]");

            var pc2 = new OhlcContext(new AssetPair(Ctx.AssetIntermediary, Ctx.Pair.Asset2), Ctx.TimeResolution, range, L);
            var d2 = Ctx.CurrencyConversionApiProvider.GetOhlc(pc2);

            if (d1.IsEmpty() || d2.IsEmpty())
                return null;

            if (d1.Count != d2.Count)
                return null;

            var ohcldata = new OhclData(_adapter.TimeResolution)
            {
                ConvertedFrom = Ctx.AssetIntermediary,
                Network = Ctx.PrimaryApiProvider.Network
            };

            var seriesid = OhlcResolutionDataAdapter.GetHash(Ctx.Pair, range.TimeResolution, ohcldata.Network);

            foreach (var i in d1)
            {
                var i2 = d2.FirstOrDefault(x => x.DateTimeUtc == i.DateTimeUtc);
                if (i2 == null)
                    return null;

                ohcldata.Add(new OhclEntry(seriesid, i.DateTimeUtc, Ctx)
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