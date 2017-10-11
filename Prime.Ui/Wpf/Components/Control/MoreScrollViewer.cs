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
        
        private int _currentPageSize = 0;

        public static readonly DependencyProperty pageIncrementProperty = DependencyProperty.Register("PageIncrement", typeof(int), typeof(MoreScrollViewer), new FrameworkPropertyMetadata(int.MinValue));

        public int PageIncrement
        {
            get => (int)GetValue(pageIncrementProperty);
            set => SetValue(pageIncrementProperty, value);
        }

        private void SViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            if (this.VerticalOffset >= (this.ScrollableHeight - 1))
            {
                var vm = (ICanMore)this.DataContext;
                vm?.AddRequest(_currentPageSize, PageIncrement);
            }
        }

        private void SViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.ScrollToVerticalOffset(this.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
