using System;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ChartHeaderViewModel : VmBase
    {
        private readonly PriceChartPaneModel _parentModel;

        public ChartHeaderViewModel(PriceChartPaneModel parentModel)
        {
            _parentModel = parentModel;
            _parentModel.OnDataUpdate += OnParentModelOnDataUpdate;
        }

        private void OnParentModelOnDataUpdate(object s, EventArgs e)
        {
            if (!(e is OhlcDataUpdatedEvent oe) || !oe.IsLive)
                return;

            var d = oe.NewData;
            if (d.IsEmpty())
                return;

            var l = d.OrderBy(x => x.DateTimeUtc).Last();
            if (l.DateTimeUtc < _lastestSoFar)
                return;

            _lastestSoFar = l.DateTimeUtc;
            LastPrice = new Money((decimal)l.Close, oe.Asset);
            LastHigh = new Money(Math.Max((decimal)l.High, LastPrice), oe.Asset);
            LastLow = new Money(Math.Min((decimal)l.Low, LastPrice), oe.Asset);
            LastVolume = l.VolumeTo;
            IsVisible = true;
        }

        private DateTime _lastestSoFar;

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        private Money _lastPrice;
        public Money LastPrice
        {
            get => _lastPrice;
            set => Set(ref _lastPrice, value);
        }

        private Money _lastHigh;
        public Money LastHigh
        {
            get => _lastHigh;
            set => Set(ref _lastHigh, value);
        }

        private Money _lastLow;
        public Money LastLow
        {
            get => _lastLow;
            set => Set(ref _lastLow, value);
        }

        private double _lastVolume;
        public double LastVolume
        {
            get => _lastVolume;
            set => Set(ref _lastVolume, value);
        }

        private TimeResolution _resolution;
        public TimeResolution Resolution
        {
            get => _resolution;
            set => Set(ref _resolution, value);
        }
    }
}