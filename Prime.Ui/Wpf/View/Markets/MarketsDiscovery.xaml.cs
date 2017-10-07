using Prime.Ui.Wpf.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Prime.Ui.Wpf.View.Markets
{
    /// <summary>
    /// Interaction logic for MarketsDiscovery.xaml
    /// </summary>
    public partial class MarketsDiscovery : UserControl
    {
        public MarketsDiscovery()
        {
            InitializeComponent();
            SViewer.PreviewMouseWheel += SViewer_PreviewMouseWheel;
            SViewer.ScrollChanged += SViewerOnScrollChanged;
        }

        private void SViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            if (SViewer.VerticalOffset >= (SViewer.ScrollableHeight - 1))
            {
                MarketsDiscoveryViewModel vm = (MarketsDiscoveryViewModel)this.DataContext;
                vm.LoadManyControls(7);
            }
        }

        private void SViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
