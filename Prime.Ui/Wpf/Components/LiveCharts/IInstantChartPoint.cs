using NodaTime;

namespace Prime.Ui.Wpf
{
    public interface IInstantChartPoint
    {
        Instant X { get; set; }
    }
}