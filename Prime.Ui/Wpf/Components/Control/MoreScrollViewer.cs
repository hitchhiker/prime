using Prime.Ui.Wpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Prime.Ui.Wpf.Components.Control
{
    public class MoreScrollViewer : ScrollViewer
    {
        public MoreScrollViewer()
        {
            this.PreviewMouseWheel += SViewer_PreviewMouseWheel;
            this.ScrollChanged += SViewerOnScrollChanged;
        }
        
        private int _currentPageIndex;

        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(MoreScrollViewer), new FrameworkPropertyMetadata(int.MinValue));

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        private void SViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            if (!(VerticalOffset >= ScrollableHeight - 1))
                return;

            var vm = DataContext as ICanMore;
            vm?.AddRequest(_currentPageIndex++, PageSize);
        }

        private void SViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollToVerticalOffset(VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
