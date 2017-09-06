using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using NodaTime;
using Prime.Core;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PriceChartPaneModel : DocumentPaneViewModel, IPaneProvider
    {
        private readonly OhlcDataAdapter _adapter;
        private readonly Dispatcher _dispatcher;
        private readonly IMessenger _messenger;
        private readonly ScreenViewModel _screenViewModel;
        public ChartGroupViewModel ChartGroupViewModel { get; private set; }
        private readonly DebounceDispatcher _debouncer;
        private readonly AssetPair _pair;
        private ChartViewModel _volumeChart;
        private ChartViewModel _priceChart;
        private readonly object _lock = new object();
        private CoverageMapMemory _renderedCoverage = new CoverageMapMemory();

        private readonly List<ZoomBaseComponent> _chartZooms = new List<ZoomBaseComponent>();
        private readonly List<ZoomBaseComponent> _allZooms = new List<ZoomBaseComponent>();
        private readonly OverviewChartZoomComponent _overviewZoom;
        private ResolutionSourceProvider _chartResolutionProvider;

        public readonly TimeResolution OverviewDefaultResolution = TimeResolution.Day;
        public readonly TimeResolution ReceiverDefaultResolution = TimeResolution.Day;

        public EventHandler OnRangeChange;

        public PriceChartPaneModel() { }

        public PriceChartPaneModel(IMessenger messenger, ScreenViewModel screenViewModel, AssetPair pair)
        {
            _dispatcher = Application.Current.Dispatcher;
            _debouncer = new DebounceDispatcher(_dispatcher);
            _messenger = messenger;
            _screenViewModel = screenViewModel;
            _pair = pair;

            Key = _pair.ToString();
            Name = _pair.ToString();
            CanClose = true;
            IsActive = true;
            IsSelected = true;

            var ctx = new OhlcResolutionAdapterAdapterContext()
            {
                Pair = _pair,
                RequestFullDaily = true,
                StatusEntry = (s) => _dispatcher.Invoke(() => DataStatus = s)
            };

            _adapter = new OhlcDataAdapter(ctx);

            _overviewZoom = new OverviewChartZoomComponent(OverviewDefaultResolution, _dispatcher);
            _allZooms.Add(_overviewZoom);

            ChartGroupViewModel = new ChartGroupViewModel(this, _messenger, _overviewZoom)
            {
                ResolutionSelected = ReceiverDefaultResolution
            };
        }

        private string _dataStatus;
        public string DataStatus
        {
            get => _dataStatus;
            set => Set(ref _dataStatus, value);
        }

        public bool IsDataBusy
        {
            get => _isDataBusy;
            set => Set(ref _isDataBusy, value);
        }

        public bool IsFor(CommandBase command)
        {
            return command is AssetGoCommand;
        }

        public DocumentPaneViewModel GetInstance(IMessenger messenger, ScreenViewModel model, CommandBase command)
        {
            var c = command as AssetGoCommand;
            var pair = new AssetPair(c.Asset, UserContext.Current.BaseAsset);
            var pcp = new PriceChartPaneModel(messenger, model, pair);
            pcp.QueueWork(pcp.InitDataThread);
            return pcp;
        }

        private void InitDataThread()
        {
            SetDataStatus("Requesting Data");
            _adapter.Init();
            SetDataStatus();

            var range = new TimeRange(-ReceiverDefaultResolution.GetDefaultTimeSpan(), ReceiverDefaultResolution);

            // Get the data for the chart from the datasource

            SetDataStatus("Requesting Data");

            var priceDataNative = _adapter.Request(range);
            if (priceDataNative == null)
            {
                SetDataStatus("Data missing", false);
                return;
            }

            _renderedCoverage.Include(range, priceDataNative);

            SetDataStatus();

            SetDataStatus("Initialising chart");

            _dispatcher.Invoke(delegate
            {
                InitData(priceDataNative);
                SetupEvents();

                SetDataStatus();
            });

            ChartGroupViewModel.PropertyChanged += delegate(object o, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(ChartGroupViewModel.ResolutionSelected))
                    QueueWork(UpdateFromResolutionChange);
            };
        }

        private void SetupEvents()
        {
            foreach (var zoom in _allZooms)
            {
                zoom.OnRangePreviewChange += (s, e) =>
                {
                    _debouncer.Debounce(25, _ =>
                    {
                        OnRangeChange?.Invoke(this, EventArgs.Empty);
                        QueueWork(UpdateData);
                    });

                    var sender = s as ZoomBaseComponent;
                    foreach (var oz in _allZooms)
                    {
                        if (oz == sender)
                            continue;

                        oz.SuspendRangeEventTill = DateTime.UtcNow.AddMilliseconds(200);
                        oz.ZoomToRange(sender.GetTimeRange());
                    }
                };
            }
        }

        private void InitData(OhclData sourceData)
        {
            if (!sourceData.Any() && sourceData.Count<2)
                return;

            lock (_lock)
            {
                var overView = _adapter.OverviewOhcl;

                var cz1 = new ReceiverChartZoomComponent(ReceiverDefaultResolution, _dispatcher);
                var cz2 = new ReceiverChartZoomComponent(ReceiverDefaultResolution, _dispatcher);

                _chartZooms.Add(cz1);
                _chartZooms.Add(cz2);

                _allZooms.AddRange(_chartZooms);

                var startpoint = Instant.FromDateTimeUtc(overView.Min(x => x.DateTimeUtc));
                var endpoint = Instant.FromDateTimeUtc(overView.Max(x => x.DateTimeUtc));
                var range = sourceData.GetTimeRange(ChartResolution);

                foreach (var z in _allZooms)
                {
                    z.StartPoint = startpoint;
                    z.EndPoint = endpoint;
                    z.ZoomToRange(range);
                }

                var chartResolver1 = _chartResolutionProvider = new ResolutionSourceProvider(() => cz1.Resolution);
                var chartResolver2= _chartResolutionProvider = new ResolutionSourceProvider(() => cz1.Resolution);

                // volume 
                var volchart = _volumeChart = new ChartViewModel(ChartGroupViewModel, cz1, false);
                volchart.SeriesCollection.Add(sourceData.ToVolumeSeries(chartResolver1, "Volume"));
                volchart.YAxesCollection.Add(GetYAxis("Volume"));

                // prices / scroller

                var priceChart = _priceChart = new ChartViewModel(ChartGroupViewModel, cz2);
                priceChart.YAxesCollection.Add(GetYAxis("Price"));
                
                priceChart.SeriesCollection.Add(sourceData.ToGCandleSeries(chartResolver2, "Prices"));
                priceChart.SeriesCollection.Add(sourceData.ToSmaSeries(50, chartResolver2));

                priceChart.CreateTruncatedVisualElement(ChartGroupViewModel.OverviewZoom.EndPoint, 1000);

                ChartGroupViewModel.ScrollSeriesCollection.Add(overView.ToScrollSeries());
                ChartGroupViewModel.Charts.Add(volchart);
                ChartGroupViewModel.Charts.Add(priceChart);
            }
        }

        private void UpdateFromResolutionChange()
        {
            lock (_lock)
            {
                var newres = ChartGroupViewModel.ResolutionSelected;
                var resetZoom = false;

                TimeRange useRange = null;

                if (!_overviewZoom.CanFit(newres))
                {
                    var ts = _overviewZoom.Resolution.GetDefaultTimeSpan();
                    useRange = new TimeRange(_overviewZoom.EndPoint.ToDateTimeUtc(), -ts, newres);
                    resetZoom = true;
                }
                else
                    useRange = _overviewZoom.GetTimeRange();

                useRange.TimeResolution = newres;

                SetDataStatus("Requesting Data");

                var nPriceData = _adapter.Request(useRange);
                if (nPriceData == null)
                {
                    SetDataStatus("Data missing", false);
                    return;
                }

                SetDataStatus();

                _dispatcher.Invoke(() =>
                {
                    ClearData();

                    foreach (var cz in _chartZooms)
                        cz.Resolution = ChartGroupViewModel.ResolutionSelected;

                    MergeData(nPriceData);

                    foreach (var cz in _chartZooms)
                    {
                        cz.SuspendRangeEventTill = DateTime.UtcNow.AddMilliseconds(200);
                        cz.ZoomToRange(useRange);
                    }

                    if (resetZoom)
                    {
                        _overviewZoom.SuspendRangeEventTill = DateTime.UtcNow.AddMilliseconds(200);
                        _overviewZoom.ZoomToRange(_overviewZoom.GetDefaultTimeRange());
                    }
                });
            }
        }

        private void UpdateData()
        {
            lock (_lock)
            {
                var r = _chartZooms.FirstOrDefault().GetTimeRange();

                if (_renderedCoverage.Covers(r))
                    return;

                SetDataStatus("Requesting Data");
                var nPriceData = _adapter.Request(r);
                if (nPriceData == null)
                {
                    SetDataStatus("Data missing", false);
                    return;
                }

                _renderedCoverage.Include(r, nPriceData);

                SetDataStatus();
                _dispatcher.Invoke(() => MergeData(nPriceData));
            }
        }

        private void ClearData()
        {
            _renderedCoverage = new CoverageMapMemory();

            // volume 
            _priceChart.SeriesCollection[0].Values.Clear();
            _volumeChart.SeriesCollection[0].Values.Clear();
        }

        private void MergeData(OhclData sourceData)
        {
            MergeSeriesViews<OhlcInstantChartPoint>(_priceChart.SeriesCollection[0], sourceData.ToGCandleSeries(_chartResolutionProvider, "Prices"));
            MergeSeriesViews<InstantChartPoint>(_volumeChart.SeriesCollection[0], sourceData.ToVolumeSeries(_chartResolutionProvider, "Volume"));
            //Merge(_priceChart.SeriesCollection[1], sourceData.ToSmaSeries(50));
        }

        private void MergeSeriesViews<T>(ISeriesView oldData, ISeriesView newData) where T: IInstantChartPoint
        {
            var ov = oldData.Values.OfType<T>().ToList();
            foreach (var nd in newData.Values.OfType<T>())
            {
                if (ov.All(x => x.X != nd.X))
                    oldData.Values.Add(nd);
            }
        }

        private Axis GetYAxis(string title)
        {
            return new Axis
            {
                // Title is with combined series
                Title = title,
                LabelFormatter = LabelFormatter,
                Position = AxisPosition.RightTop,
                MinRange = 0,
                MinValue = 0,
                Sections = new SectionsCollection
                {
                    // Horizontal 0 value line
                    new AxisSection
                    {
                        ToolTip = "Hello",
                        Value = 0,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1
                    }/*,
                    new AxisSection
                    {
                        ToolTip = "Now",
                        Value = 2700,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        Fill = new SolidColorBrush
                        {
                            Color = System.Windows.Media.Color.FromRgb(254,132,132),
                            Opacity = .4
                        }
                    }*/
                }
            };
        }

        public void QueueWork(Action action)
        {
            PrimeWpf.I.STAThreadPool.QueueWorkItem(a =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
#if DEBUG
                    DataStatus = e.Message;
#else
                    DataStatus = "Fatal error";
#endif
                }
                return null;
            });
        }

        public TimeResolution ChartResolution => _chartZooms.FirstOrDefault().Resolution;

        private int _padLength = 10;
        private bool _isDataBusy;

        private string LabelFormatter(double v)
        {
            var s = v.ToString();
            s = s.PadLeft(_padLength, ' ');
            return s;
        }

        public void SetDataStatus(string message = null, bool? busy = null)
        {
            if (message == null)
            {
                IsDataBusy = busy ?? false;
                DataStatus = "Idle";
            }
            else
            {
                IsDataBusy = busy ?? true;
                DataStatus = message;
            }
        }

        public override CommandContent Create()
        {
            return new AssetGoCommand(_pair.Asset1);
        }
    }
}