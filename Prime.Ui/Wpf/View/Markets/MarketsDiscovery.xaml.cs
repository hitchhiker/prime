using System;
using System.Runtime.InteropServices;
using System.Windows;
using Prime.Ui.Wpf.ViewModel;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
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

        private static int _pageIncrement = 2;

        private int _currentPageSize = 0;

        private void SViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            if (SViewer.VerticalOffset >= (SViewer.ScrollableHeight - 1))
            {
                var vm = (ICanMore)this.DataContext;
                vm?.AddRequest(_currentPageSize, _pageIncrement);
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
