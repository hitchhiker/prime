using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Core
{
    public class OhlcDataAdapter
    {
        private readonly OhlcResolutionAdapter _adapterMinute;
        private readonly OhlcResolutionAdapter _adapterHour;
        private readonly OhlcResolutionAdapter _adapterDay;
        public readonly OhlcResolutionContext Ctx;

        public OhlcDataAdapter(AssetPair pair) : this(new OhlcResolutionContext() {Pair = pair}) {}
        
        public OhlcDataAdapter(OhlcResolutionContext adapterContext)
        {
            Ctx = adapterContext;

            _adapters.Add(_adapterMinute = new OhlcResolutionAdapter(new OhlcResolutionAdapterContext(Ctx) { TimeResolution = TimeResolution.Minute}));
            _adapters.Add(_adapterHour = new OhlcResolutionAdapter(new OhlcResolutionAdapterContext(Ctx) { TimeResolution = TimeResolution.Hour}));
            _adapters.Add(_adapterDay = new OhlcResolutionAdapter(new OhlcResolutionAdapterContext(Ctx) { TimeResolution = TimeResolution.Day}));
        }

        public DateTime UtcDataStart { get; set; }

        public OhlcData OverviewOhlc { get; set; }

        public IReadOnlyList<OhlcResolutionAdapter> Adapters => _adapters;

        private readonly List<OhlcResolutionAdapter> _adapters = new List<OhlcResolutionAdapter>();

        private volatile bool _isInit;

        public void Init()
        {
            if (_isInit)
                return;

            _isInit = true;

            if (Ctx.ApiEnabled)
                InitApiProviders();
            
            if (Ctx.RequestFullDaily)
                RequestFullDaily();
        }

        private void InitApiProviders()
        {
            Ctx.PrimaryApiProvider = Ctx.PrimaryApiProvider ?? Ctx.GetDefaultAggregationProvider();

            if (Ctx.RequiresApiDiscovery())
                Ctx.DiscoverAndApplyApiProviders();

            _adapters.ForEach(x =>
            {
                x.Ctx.CanDiscoverApiProviders = false;
                x.Ctx.PrimaryApiProvider = Ctx.PrimaryApiProvider;
                x.Ctx.CurrencyConversionApiProvider = Ctx.CurrencyConversionApiProvider;
            });

            Ctx.EnsureProvider();
        }

        private void RequestFullDaily()
        {
            var range = TimeRange.EveryDayTillNow;
            OverviewOhlc = Request(range, true);

            if (OverviewOhlc.IsEmpty())
                throw new Exception("Data range missing during " + nameof(Init));

            UtcDataStart = OverviewOhlc.Min(x => x.DateTimeUtc);
        }

        private readonly object _requestLock = new object();

        public OhlcData Request(TimeRange timeRange, bool allowStale = false)
        {
            lock (_requestLock)
            {
                switch (timeRange.TimeResolution)
                {
                    case TimeResolution.Day:
                        return _adapterDay.Request(timeRange, allowStale);
                    case TimeResolution.Hour:
                        return _adapterHour.Request(timeRange, allowStale);
                    case TimeResolution.Minute:
                        return _adapterMinute.Request(timeRange, allowStale);
                    default:
                        return OhlcData.Empty;
                }
            }
        }
    }
}
