using System;
using System.Windows.Threading;
using LiveCharts.Dtos;
using NodaTime;
using Prime.Core;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Chart component responsible for the zoom state
    /// </summary>
    public abstract class ZoomBaseComponent : VmBase
    {
        protected readonly ZoomBaseComponent _zoomChart;
        protected readonly DebounceDispatcher _debounceDispatcher;
        protected readonly Dispatcher _uiDispatcher;

        protected double _zoomFrom;
        protected double _zoomTo = 1;
        protected int RangeDebounce = 5;
       
        public DateTime SuspendRangeEventTill = DateTime.MinValue;

        public double ZoomToLimit => EndPoint.ToUnixTimeTicks() / AxisModifier;
        public virtual double ZoomFromLimit => StartPoint.ToUnixTimeTicks() / AxisModifier;

        public bool IsPositionLocked { get; set; }

        public double AxisModifier => Resolution.GetAxisModifier();

        public DateTime UnixEpochUtc => DateTimeExt.UnixEpoch;
        public DateTime StartPointUtc => StartPoint.ToDateTimeUtc();
        public DateTime EndPointUtc => EndPoint.ToDateTimeUtc();

        public Instant StartPoint
        {
            get => _startPoint;
            set => Set(ref _startPoint, value);
        }

        public Instant EndPoint => Instant.FromDateTimeUtc(DateTime.UtcNow);
        
        public TimeResolution Resolution
        {
            get => _resolution;
            set => Set(ref _resolution, value);
        }

        public Action<CorePoint, bool> ZoomProxy { get; set; }

        public bool IsMouseOver { get; set; }

        protected ZoomBaseComponent(TimeResolution resolution, Dispatcher uiDispatcher)
        {
            Resolution = resolution;
            _debounceDispatcher = new DebounceDispatcher(uiDispatcher);
        }

        public EventHandler OnRangePreviewChange;

        protected double LastFrom;
        protected double LastTo;
        private TimeResolution _resolution = TimeResolution.None;
        private Instant _endPoint = Instant.FromDateTimeUtc(DateTime.UtcNow);
        private Instant _startPoint = Instant.FromUnixTimeSeconds(0);

        public abstract double ZoomFrom { get; set; }

        public abstract double ZoomTo { get; set; }

        public bool CanRangeEvent() => SuspendRangeEventTill <= DateTime.UtcNow;

        public bool IsNearRightEdge()
        {
            var vport = _zoomTo - _zoomFrom;
            var prox = ZoomToLimit - _zoomTo;
            var max = vport * .1; // 10% lock tolerance
            var isclose = prox < max;
            return isclose;
        }

        public void Update(bool withRangeUpdate = false)
        {
            if (!IsNearRightEdge())
                return;

            var adjust = ZoomToLimit - _zoomTo;
            ZoomFrom += adjust;

            if (withRangeUpdate)
                ZoomTo += adjust;
            else
            {
                UpdateRange(_zoomTo + adjust, true);
                this.RaisePropertyChanged(nameof(ZoomTo));
            }
        }

        protected abstract void UpdateRange(double zoomTo, bool skipRangeTrigger = false);

        protected bool ForceOneRangeUpdate { get; set; }

        public TimeRange GetDefaultTimeRange()
        {
            var ts = Resolution.GetDefaultTimeSpan();
            return new TimeRange(EndPoint.ToDateTimeUtc(), -ts, Resolution);
        }

        public virtual void ZoomToDefault()
        {
            ZoomToRange(GetDefaultTimeRange());
        }

        public virtual void ZoomToRange(TimeRange range)
        {
            ZoomFrom = Instant.FromDateTimeUtc(range.UtcFrom).ToUnixTimeTicks() / AxisModifier;
            ZoomTo = Instant.FromDateTimeUtc(range.UtcTo).ToUnixTimeTicks() / AxisModifier;
            RaiseZoomProperyChanges();
        }

        protected void RaiseZoomProperyChanges()
        {
            this.RaisePropertyChanged(nameof(ZoomFrom));
            this.RaisePropertyChanged(nameof(ZoomTo));
        }

        public virtual TimeRange GetTimeRange(TimeResolution? resolution = null)
        {
            var res = resolution ?? Resolution;
            var from = Instant.FromUnixTimeTicks((long)(ZoomFrom * AxisModifier));
            var to = Instant.FromUnixTimeTicks((long)(ZoomTo * AxisModifier));
            return new TimeRange(from.ToDateTimeUtc(), to.ToDateTimeUtc(), res);
        }
    }
}