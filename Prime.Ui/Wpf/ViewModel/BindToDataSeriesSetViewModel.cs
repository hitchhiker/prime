using System.Windows.Input;
using SciChart.Charting.Common.Helpers;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.ViewportManagers;
using SciChart.Examples.ExternalDependencies.Common;
using SciChart.Examples.ExternalDependencies.Data;

namespace Prime.Ui.Wpf.ViewModel
{
    public class BindToDataSeriesSetViewModel : BaseViewModel
    {
        private IDataSeries<double, double> _dataSeries0;
        private readonly RandomWalkGenerator _dataSource;

        public BindToDataSeriesSetViewModel()
        {
            ViewportManager = new DefaultViewportManager();

            // Create a DataSeriesSet
            _dataSeries0 = new XyDataSeries<double, double>();

            // Create a single data-series
            _dataSource = new RandomWalkGenerator();
            var data = _dataSource.GetRandomWalkSeries(1000);

            // Append data to series.
            _dataSeries0.Append(data.XData, data.YData);
        }

        // Databound to via SciChartSurface.DataSet in the view
        public IDataSeries<double, double> PriceData
        {
            get { return _dataSeries0; }
            set
            {
                _dataSeries0 = value;
                OnPropertyChanged("PriceData");
            }
        }

        public IViewportManager ViewportManager { get; set; }

        public ICommand AppendDataCommand
        {
            get { return new ActionCommand(AppendData); }
        }

        // Called when the AppendDataCommand is invoked via button click on the view
        private void AppendData()
        {
            var newData = _dataSource.GetRandomWalkSeries(50);

            _dataSeries0.Append(newData.XData, newData.YData);
            ViewportManager.ZoomExtents();
        }
    }
}
