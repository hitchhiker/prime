using System;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.IconPacks;
using NodaTime;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ChartViewModel : VmBase
    {
        private ReceiverZoomViewModel _zoom;
        private SeriesCollection _seriesCollection = new SeriesCollection();
        private AxesCollection _yAxesCollection = new AxesCollection();
        private VisualElementsCollection _visualElementsCollection = new VisualElementsCollection();
        private ChartGroupViewModel _parent;

        private bool _showX;

        public Dictionary<string, Instant> LastUpdates { get; } = new Dictionary<string, Instant>();

        public ChartViewModel() { }

        public ChartViewModel(ChartGroupViewModel chart, ReceiverZoomViewModel zoom, bool showX = true)
        {
            ShowX = showX;
            _parent = chart;
            _zoom = zoom;
        }

        public ChartGroupViewModel Parent
        {
            get => _parent;
            set => Set(ref _parent, value);
        }

        public ReceiverZoomViewModel Zoom
        {
            get => _zoom;
            set => Set(ref _zoom, value);
        }

        public AxesCollection YAxesCollection
        {
            get => _yAxesCollection;
            set => Set(ref _yAxesCollection, value);
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set => Set(ref _seriesCollection, value);
        }

        public VisualElementsCollection VisualElementsCollection
        {
            get => _visualElementsCollection;
            set => Set(ref _visualElementsCollection, value);
        }

        public bool ShowX
        {
            get => _showX;
            set => Set(ref _showX, value);
        }

        public void CreateTruncatedVisualElement(Instant x, decimal y)
        {
            _visualElementsCollection.Add(new VisualElement
            {
                X = x.ToUnixTimeTicks() / Zoom.AxisModifier,
                Y = (double)y,
                UIElement = new PackIconMaterialLight
                {
                    ToolTip = $"This series is possibly truncated.",
                    Width = 16,
                    Height = 16,
                    Kind = PackIconMaterialLightKind.Alert
                }
            });
        }

        public bool ShowLabels => ShowX;

        public bool ShowLegend => false;
    }
}