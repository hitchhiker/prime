using System.Windows;
using System.Windows.Controls;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for LogPanel.xaml
    /// </summary>
    public partial class LogPanel : UserControl
    {
        public LogPanel()
        {
            InitializeComponent();
        }

        private bool _autoScroll = true;

        /// <summary>
        /// https://stackoverflow.com/a/34123691/1318333
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var changed = e.ExtentHeightChange != 0;
            var sv = (e.Source as ScrollViewer);

            if (!changed)
                _autoScroll = sv.VerticalOffset == sv.ScrollableHeight;
            
            if (_autoScroll && changed)
                sv.ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
        }
    }
}
