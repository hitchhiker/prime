using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Events;
using LiveCharts.Wpf;
using NodaTime;
using Prime.Common;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;
using Series = LiveCharts.Wpf.Series;

namespace Prime.Ui.Wpf.ViewModel
{
    /// <summary>
    /// View model for generic charts
    /// </summary>
    public class ChartGroupViewModel : VmBase, IResolutionSource
    {
        public ChartGroupViewModel() { }

        public ChartGroupViewModel(PriceChartPaneViewModel parentViewModel, OverviewZoomViewModel zoom)
        {
            ParentViewModel = parentViewModel;

            OverviewZoom = zoom;
            ZoomResetCommand = new RelayCommand(OverviewZoom.ZoomToDefault);
            _resolutionSelected = zoom.Resolution;
            ChartHeaderViewModel = new ChartHeaderViewModel(parentViewModel);
            ParentViewModel.OnRangeChange += (s, e) => InvalidateRangeProperties();
        }

        public PriceChartPaneViewModel ParentViewModel { get; private set; }

        private DateTime _initialDateTime = Instant.FromUnixTimeSeconds(0).ToDateTimeUtc();
        private TimeResolution _resolutionSelected;
        private bool _isPositionLocked;
        private ObservableCollection<ChartViewModel> _chartViewModels = new ObservableCollection<ChartViewModel>();
        private SeriesCollection _scrollSeriesCollection = new SeriesCollection();

        public ChartHeaderViewModel ChartHeaderViewModel { get; }

        /// <inheritdoc />
        public bool IsPositionLocked
        {
            get => _isPositionLocked;
            set => Set(ref _isPositionLocked, value);
        }

        public TimeResolution ResolutionSelected
        {
            get => _resolutionSelected;
            set => SetAfter(ref _resolutionSelected, value, (a) => { InvalidateRangeProperties(); });
        }

        public void InvalidateRangeProperties()
        {
            RaisePropertyChanged(nameof(IsDaily));
            RaisePropertyChanged(nameof(IsHourly));
            RaisePropertyChanged(nameof(IsMinute));
            RaisePropertyChanged(nameof(CanDaily));
            RaisePropertyChanged(nameof(CanMinute));
            RaisePropertyChanged(nameof(CanHourly));
            RaisePropertyChanged(nameof(IsOverViewVisible));
        }

        public bool IsDaily
        {
            get => ResolutionSelected == TimeResolution.Day;
            set => ResolutionSelected = value ? TimeResolution.Day : TimeResolution.None;
        }

        public bool IsHourly
        {
            get => ResolutionSelected == TimeResolution.Hour;
            set => ResolutionSelected = value ? TimeResolution.Hour : TimeResolution.None;
        }

        public bool IsMinute
        {
            get => ResolutionSelected == TimeResolution.Minute;
            set => ResolutionSelected = value ? TimeResolution.Minute : TimeResolution.None;
        }

        public bool CanDaily
        {
            get => ResolutionSelected != TimeResolution.Day && ParentViewModel.IsGraphReady;
            set { }
        }

        public bool CanHourly
        {
            get => ResolutionSelected != TimeResolution.Hour && ParentViewModel.IsGraphReady;
            set { }
        }

        public bool CanMinute
        {
            get => ResolutionSelected != TimeResolution.Minute && ParentViewModel.IsGraphReady;
            set { }
        }

        /// <summary>
        /// Gets the DateTime representing the first X value
        /// </summary>
        public DateTime InitialDateTime
        {
            get => _initialDateTime;
            protected set => Set(ref _initialDateTime, value);
        }

        public ObservableCollection<ChartViewModel> Charts
        {
            get => _chartViewModels;
            set => Set(ref _chartViewModels, value);
        }

        public SeriesCollection ScrollSeriesCollection
        {
            get => _scrollSeriesCollection;
            set => Set(ref _scrollSeriesCollection, value);
        }

        public bool IsOverViewVisible => ResolutionSelected != TimeResolution.Minute;

        public RelayCommand ZoomResetCommand { get; private set; }

        public RelayCommand<RangeChangedEventArgs> RangeChangedCommand { get; private set; }

        public OverviewZoomViewModel OverviewZoom { get; private set; }

        TimeResolution IResolutionSource.Resolution { get; set; }
    }
}