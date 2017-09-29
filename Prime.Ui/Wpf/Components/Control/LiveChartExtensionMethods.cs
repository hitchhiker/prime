using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using NodaTime;
using Prime.Core;

namespace Prime.Ui.Wpf
{
    public static class LiveChartExtensionMethods
    {
        public static GCandleSeries ToGCandleSeries(this OhlcData data, ResolutionSourceProvider resolver = null, string title = "")
        {
            var ohlcChartPointEvaluator = new OhlcInstantChartPointMapper(resolver ?? new ResolutionSourceProvider(data.Resolution));

            var series = new GCandleSeries
            {
                Configuration = ohlcChartPointEvaluator,
                IncreaseBrush = Brushes.Aquamarine,
                DecreaseBrush = Brushes.LightCoral,
                Fill = Brushes.Transparent,
                Title = title
            };

            if (data == null)
                return series;

            var values = new GearedValues<OhlcInstantChartPoint>();
            values.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new OhlcInstantChartPoint(i)));

            series.Values = values;

            return series;
        }

        public static GLineSeries ToScrollSeries(this OhlcData data, ResolutionSourceProvider resolver = null, string title = "")
        {
            var chartPointEvaluator = new InstantChartPointMapper(resolver ?? new ResolutionSourceProvider(data.Resolution));

            var series = new GLineSeries
            {
                Configuration = chartPointEvaluator,
                PointGeometry = null,
                Title = title
            };

            if (data == null)
                return series;

            var values = new GearedValues<InstantChartPoint>();
            values.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new InstantChartPoint(i)));
            series.Values = values;

            return series;
        }

        public static GColumnSeries ToVolumeSeries(this OhlcData data, ResolutionSourceProvider resolver = null, string title = "")
        {
            var chartPointEvaluator = new InstantChartPointMapper(resolver ?? new ResolutionSourceProvider(data.Resolution));

            var series = new GColumnSeries
            {
                Configuration = chartPointEvaluator,
                Title = title
            };

            if (data == null)
                return series;

            var values = new GearedValues<InstantChartPoint>();
            values.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new InstantChartPoint(i)));
            values.Quality = Quality.Low;
            series.Values = values;

            return series;
        }

        public static GLineSeries ToSmaSeries(this OhlcData data, int length, ResolutionSourceProvider resolver = null, string title = null)
        {
            title = title ?? length + " SMA";

            var chartPointEvaluator = new InstantChartPointMapper(resolver ?? new ResolutionSourceProvider(data.Resolution));

            var series = new GLineSeries
            {
                Configuration = chartPointEvaluator,
                Fill = Brushes.Transparent,
                PointGeometry = null,
                Title = title
            };

            if (data == null)
                return series;

            var values = new GearedValues<InstantChartPoint>();
            var ordered = data.OrderBy(x => x.DateTimeUtc).ToList();
            var smadata = FinancialHelper.ComputeMovingAverage(ordered.Select(x => x.Close).ToList(), length);
            for (var index = 0; index < ordered.Count; index++)
            {
                var d = ordered[index];
                var v = smadata[index];
                if (double.IsNaN(v))
                    continue;
                values.Add(new InstantChartPoint(d.DateTimeUtc, (decimal) v));
            }

            series.Values = values;

            return series;
        }
    }
}
