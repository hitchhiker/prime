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
        public readonly OverviewZoomComponent OverviewZoom;
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

            OverviewZoom = new OverviewZoomComponent(OverviewDefaultResolution, _dispatcher);

            _allZooms.Add(OverviewZoom);

            ChartGroupViewModel = new ChartGroupViewModel(this, _messenger, OverviewZoom)
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
                CreateCharts(priceDataNative);
                SetupZoomEvents();
                SetDataStatus();
            });

            ChartGroupViewModel.PropertyChanged += delegate(object o, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(ChartGroupViewModel.ResolutionSelected))
                    QueueWork(UpdateFromResolutionChange);
            };
        }

        private void SetupZoomEvents()
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
                        oz.ZoomToRange(sender.GetTimeRange(oz.Resolution));
                    }
                };
            }
        }

        private void CreateCharts(OhclData sourceData)
        {
            if (!sourceData.Any() && sourceData.Count<2)
                return;

            lock (_lock)
            {
                var overView = _adapter.OverviewOhcl;

                var receiverZoom = new ReceiverZoomComponent(ReceiverDefaultResolution, _dispatcher);

                _chartZooms.Add(receiverZoom);
                _allZooms.AddRange(_chartZooms);

                var startpoint = Instant.FromDateTimeUtc(overView.Min(x => x.DateTimeUtc));
                var endpoint = Instant.FromDateTimeUtc(DateTime.UtcNow); //Instant.FromDateTimeUtc(overView.Max(x => x.DateTimeUtc));
                var range = sourceData.GetTimeRange(ChartResolution);

                foreach (var z in _allZooms)
                {
                    z.StartPoint = startpoint;
                    z.EndPoint = endpoint;
                    z.ZoomToRange(range);
                }

                var resolver = _chartResolutionProvider = new ResolutionSourceProvider(() => receiverZoom.Resolution);

                // volume 

                var volchart = _volumeChart = new ChartViewModel(ChartGroupViewModel, receiverZoom, false);
                volchart.SeriesCollection.Add(sourceData.ToVolumeSeries(resolver, "Volume"));
                volchart.YAxesCollection.Add(GetYAxis("Volume"));

                // prices / scroller

                var priceChart = _priceChart = new ChartViewModel(ChartGroupViewModel, receiverZoom);
                priceChart.YAxesCollection.Add(GetYAxis("Price"));
                
                priceChart.SeriesCollection.Add(sourceData.ToGCandleSeries(resolver, "Prices"));

                //priceChart.SeriesCollection.Add(sourceData.ToSmaSeries(50, chartResolver2));

                ChartGroupViewModel.ScrollSeriesCollection.Add(overView.ToScrollSeries());
                ChartGroupViewModel.Charts.Add(volchart);
                ChartGroupViewModel.Charts.Add(priceChart);

                OverviewZoom.SetStartFrom(overView.MinOrDefault(x=>x.DateTimeUtc, DateTime.MinValue));
            }
        }

        private void UpdateFromResolutionChange()
        {
            lock (_lock)
            {
                var newres = ChartGroupViewModel.ResolutionSelected;

                OverviewZoom.SetStartFrom(newres);
                
                TimeRange resetZoom = null;
                TimeRange newRange = null;

                //if (!OverviewZoom.CanFit(newres))
                //{
                    var ts = newres.GetDefaultTimeSpan();
                    var ep = OverviewZoom.EndPoint.ToDateTimeUtc();
                /*switch (newres)
                {
                    case TimeResolution.Hour:
                        ep = ep.AddHours(23);
                        break;
                    case TimeResolution.Minute:
                        ep = ep.AddHours(23).AddMinutes(59);
                        break;
                }*/
                    newRange = new TimeRange(ep, -ts, newres);
                    resetZoom = new TimeRange(newRange.UtcFrom, newRange.UtcTo, OverviewZoom.Resolution);
                /*}
                else
                {
                    newRange = OverviewZoom.GetTimeRange();
                    newRange.TimeResolution = newres;
                    resetZoom = newRange;
                }*/

                SetDataStatus("Requesting Data");

                var nPriceData = _adapter.Request(newRange);
                if (nPriceData == null)
                {
                    SetDataStatus("Data missing", false);
                    return;
                }

                _renderedCoverage.Clear();

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
                        cz.Resolution = newRange.TimeResolution;
                        cz.ZoomToRange(newRange);
                    }

                    if (resetZoom!=null)
                    {
                        OverviewZoom.SuspendRangeEventTill = DateTime.UtcNow.AddMilliseconds(200);
                        OverviewZoom.ZoomToRange(resetZoom);
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
                Title = title,
                LabelFormatter = LabelFormatter,
                Position = AxisPosition.RightTop,
                Sections = new SectionsCollection
                {
                    // Horizontal 0 value line
                    new AxisSection
                    {
                        Value = 0,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1
                    }
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