using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Dtos;
using LiveCharts.Wpf.Charts.Base;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for ChartPanel.xaml
    /// </summary>
    public partial class ChartPanel : UserControl
    {
        private readonly Debouncer _dispatcher;
        private PriceChartPaneViewModel _pcmodel;
        public ChartPanel()
        {
            InitializeComponent();
            _dispatcher = new Debouncer();
            this.DataContextChanged += ChartPanel_DataContextChanged;

        }

        private void ChartPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _pcmodel = DataContext as PriceChartPaneViewModel;
            if (_pcmodel == null)
                return;

            //Overview.MouseUp += Overview_MouseUp;

            Overview.MouseEnter += (o, args) => _pcmodel.OverviewZoom.IsMouseOver = true;
            Overview.MouseLeave += (o, args) => _pcmodel.OverviewZoom.IsMouseOver = false;

            FixMouseSticking();
        }

        private void Overview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is ChartGroupViewModel m) || e == null)
                return;

            //TODO: ScrollViewer click to reposition
        }

        /// <summary>
        /// Fix for caught mouse on scroll
        /// </summary>
        private void FixMouseSticking()
        {
            var bt = typeof(Chart);
            var sbu = bt.GetMethod("ScrollBarOnMouseUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (sbu != null)
                Overview.MouseLeave += delegate
                {
                   sbu.Invoke(Overview, new object[] { null, null });
                };
        }

        private void Overview_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            _dispatcher.Debounce(10, o => Debounced(o as MouseWheelEventArgs), e);
        }

        private void Debounced(MouseWheelEventArgs e)
        {
            var m = _pcmodel.ChartGroupViewModel;

            var p = e.GetPosition(this);
            var corePoint = new CorePoint(p.X, p.Y);

            e.Handled = true;
            m.OverviewZoom.ZoomProxy?.Invoke(corePoint, e.Delta > 0);
        }
    }
}
