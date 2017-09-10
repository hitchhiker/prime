using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public class OhlcDataAdapter
    {
        private readonly OhlcResolutionDataAdapter _adapterMinute;
        private readonly OhlcResolutionDataAdapter _adapterHour;
        private readonly OhlcResolutionDataAdapter _adapterDay;
        public readonly OhlcResolutionAdapterAdapterContext Ctx;

        public OhlcDataAdapter(AssetPair pair) : this(new OhlcResolutionAdapterAdapterContext() {Pair = pair}) {}
        
        public OhlcDataAdapter(OhlcResolutionAdapterAdapterContext adapterContext)
        {
            Ctx = adapterContext;

            _adapters.Add(_adapterMinute = new OhlcResolutionDataAdapter(new OhlcResolutionAdapterContext(Ctx) { TimeResolution = TimeResolution.Minute}));
            _adapters.Add(_adapterHour = new OhlcResolutionDataAdapter(new OhlcResolutionAdapterContext(Ctx) { TimeResolution = TimeResolution.Hour}));
            _adapters.Add(_adapterDay = new OhlcResolutionDataAdapter(new OhlcResolutionAdapterContext(Ctx) { TimeResolution = TimeResolution.Day}));
        }

        public DateTime UtcDataStart { get; set; }

        public OhclData OverviewOhcl { get; set; }

        public IReadOnlyList<OhlcResolutionDataAdapter> Adapters => _adapters;

        private readonly List<OhlcResolutionDataAdapter> _adapters = new List<OhlcResolutionDataAdapter>();

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
            OverviewOhcl = _adapterDay.Request(range);

            if (OverviewOhcl.IsEmpty())
                throw new Exception("Data range missing during " + nameof(Init));

            UtcDataStart = OverviewOhcl.Min(x => x.DateTimeUtc);
        }

        public OhclData Request(TimeRange timeRange)
        {
            switch (timeRange.TimeResolution)
            {
                case TimeResolution.Day:
                    return _adapterDay.Request(timeRange);
                case TimeResolution.Hour:
                    return _adapterHour.Request(timeRange);
                case TimeResolution.Minute:
                    return  _adapterMinute.Request(timeRange);
                default:
                    return OhclData.Empty;
            }
        }
    }
}
