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
        public static OhlcSeries ToOhlcSeries(this OhclData data, AssetPair pair)
        {
            var vals = new ChartValues<OhlcPoint>();
            var series = new OhlcSeries() { Values = vals };

            if (data == null)
                return series;

            vals.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new OhlcPoint(i.Open, i.High, i.Low, i.Close)));
            return series;
        }

        public static GCandleSeries ToGCandleSeries(this OhclData data, ResolutionSourceProvider resolver = null, string title = "")
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
            values.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new OhlcInstantChartPoint {X = Instant.FromDateTimeUtc(i.DateTimeUtc), Open = i.Open, High = i.High, Low = i.Low, Close = i.Close}));

            series.Values = values;

            return series;
        }

        public static GLineSeries ToScrollSeries(this OhclData data, ResolutionSourceProvider resolver = null, string title = "")
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
            values.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new InstantChartPoint { X = Instant.FromDateTimeUtc(i.DateTimeUtc), Y = (decimal)i.Close }));
            series.Values = values;

            return series;
        }

        public static GColumnSeries ToVolumeSeries(this OhclData data, ResolutionSourceProvider resolver = null, string title = "")
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
            values.AddRange(data.OrderBy(x => x.DateTimeUtc).Select(i => new InstantChartPoint { X = Instant.FromDateTimeUtc(i.DateTimeUtc), Y = (decimal)i.VolumeTo }));
            values.Quality = Quality.Low;
            series.Values = values;

            return series;
        }

        public static GLineSeries ToSmaSeries(this OhclData data, int length, ResolutionSourceProvider resolver = null, string title = null)
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
                values.Add(new InstantChartPoint{X = Instant.FromDateTimeOffset(d.DateTimeUtc), Y = (decimal)v});
            }

            series.Values = values;

            return series;
        }
    }
}
