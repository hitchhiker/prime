using LiveCharts.Configurations;

namespace Prime.Ui.Wpf
{
    public abstract class CartesianMapperBase<T> : CartesianMapper<T>
    {
        protected IResolutionSource Source;

        protected CartesianMapperBase(IResolutionSource source)
        {
            Source = source;
        }
    }
}