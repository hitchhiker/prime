using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Dtos;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl
    {
        public ChartControl()
        {
            this.DataContextChanged += ChartControl_DataContextChanged;
            InitializeComponent();
        }

        private void ChartControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(DataContext is ChartViewModel cvm))
                return;

            ChartElement.MouseEnter += (o, args) => cvm.Zoom.IsMouseOver = true;
            ChartElement.MouseLeave += (o, args) => cvm.Zoom.IsMouseOver = false;

            if (cvm.Parent.OverviewZoom.ZoomProxy==null)
                cvm.Parent.OverviewZoom.ZoomProxy = (corePoint, isZoomIn) =>
                {
                    cvm.Zoom.IsMouseOver = true;
                    if (isZoomIn)
                        ChartElement.Model.ZoomIn(corePoint);
                    else
                        ChartElement.Model.ZoomOut(corePoint);
                    cvm.Zoom.IsMouseOver = false;
                };
        }
    }
}
