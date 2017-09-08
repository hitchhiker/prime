using System;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Dtos;
using LiveCharts.Wpf.Charts.Base;
using NodaTime;
using Prime.Core;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Chart component responsible for the zoom state
    /// </summary>
    public class OverviewChartZoomComponent : ZoomBaseComponent
    {
        public OverviewChartZoomComponent(TimeResolution resolution, Dispatcher uiDispatcher) : base(resolution, uiDispatcher)
        {
        }

        public override double ZoomFrom
        {
            get => _zoomFrom;
            set => _zoomFrom = value;
        }

        public override double ZoomTo
        {
            get => _zoomTo;
            set => UpdateRange(value); // this fires second, bit of a hack really.
        }

        protected void UpdateRange(double zoomTo)
        {
            //check from didn't pass left edge
            //maintain width if so.

            if (_zoomFrom < ZoomFromLimit)
            {
                zoomTo += ZoomFromLimit - _zoomFrom;
                _zoomFrom = ZoomFromLimit;
            }

            var finalv = Math.Min(ZoomToLimit, zoomTo);

            var diff = zoomTo - finalv;
            if (diff > 0)
                _zoomFrom -= diff;

            _zoomTo = finalv;

            RaisePropertyChanged(nameof(ZoomFrom));
            RaisePropertyChanged(nameof(ZoomTo));

            if (IsMouseOver && CanRangeEvent())
                OnRangePreviewChange?.Invoke(this, EventArgs.Empty);
        }

        public bool CanHourly => GetTimeRange().ToTimeSpan() <= TimeResolution.Hour.MaxTimeSpanRange();

        public bool CanMinute => GetTimeRange().ToTimeSpan() <= TimeResolution.Minute.MaxTimeSpanRange();

        public bool CanFit(TimeResolution newResolution)
        {
            var currentRange = GetTimeRange().ToTimeSpan();

            if (newResolution.IsSmallerThan(Resolution))
            {
                var max = newResolution.MaxTimeSpanRange();
                if (max >= currentRange)
                    return true;
            }
            else
            {
                var min = newResolution.MinTimeSpanRange();
                if (min <= currentRange)
                    return true;
            }
            return false;
        }
    }
}