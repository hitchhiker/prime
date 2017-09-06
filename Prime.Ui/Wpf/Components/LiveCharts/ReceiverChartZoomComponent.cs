using System;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf.Charts.Base;
using NodaTime;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public class ReceiverChartZoomComponent : ZoomBaseComponent
    {
        public ReceiverChartZoomComponent(TimeResolution resolution, Dispatcher uiDispatcher) : base(resolution, uiDispatcher)
        {
            RangeDebounce = 50;
        }

        private double _lastSentFrom;

        public override double ZoomFrom
        {
            get => _zoomFrom;
            set => _lastSentFrom = value;
        }

        public override double ZoomTo
        {
            get => _zoomTo;
            set
            {
                _zoomTo = value;
                BothExtentsUpdated();
            }
        }

        private void BothExtentsUpdated()
        {
            var finalFrom = Math.Max(ZoomFromLimit, _lastSentFrom);
            var finalTo = Math.Min(ZoomToLimit, _zoomTo);

            if (finalFrom > finalTo)
                finalFrom = ZoomFromLimit;

            if (finalTo < finalFrom)
                finalTo = ZoomToLimit;

            if (finalTo == finalFrom)
                finalFrom--;

            RangeWidthLimit(ref finalFrom, ref finalTo);

            _zoomTo = finalTo;
            _zoomFrom = finalFrom;
          
            if (LastTo!=_zoomTo)
                RaisePropertyChanged(nameof(ZoomTo));

            if (LastFrom != _zoomFrom)
                RaisePropertyChanged(nameof(ZoomFrom));

            if (CanRangeEvent() && (LastTo!=_zoomTo || LastFrom !=_zoomFrom))
                OnRangePreviewChange?.Invoke(this, EventArgs.Empty);

            LastFrom = _zoomFrom;
            LastTo = _zoomTo;
        }

        private void RangeWidthLimit(ref double from, ref double to)
        {
            var fromTicks = (long)(from * AxisModifier);
            var toTicks = (long)(to * AxisModifier);
            var diffTicks = toTicks - fromTicks;

            if (diffTicks < 0 || (diffTicks <= Resolution.MaxTimeSpanRange().Ticks && diffTicks >= Resolution.MinTimeSpanRange().Ticks))
                return;

            from = LastFrom;
            to = LastTo;
        }
    }
}