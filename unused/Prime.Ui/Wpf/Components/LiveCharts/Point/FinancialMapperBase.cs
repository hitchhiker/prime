using LiveCharts.Configurations;

namespace Prime.Ui.Wpf
{
    public abstract class FinancialMapperBase<T> : FinancialMapper<T>
    {
        protected IResolutionSource Source;

        protected FinancialMapperBase(IResolutionSource source)
        {
            Source = source;
        }
    }
}