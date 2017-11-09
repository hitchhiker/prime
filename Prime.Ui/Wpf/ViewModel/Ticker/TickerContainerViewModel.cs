using System.Collections.ObjectModel;

namespace Prime.Ui.Wpf.ViewModel
{
    public class TickerContainerViewModel : VmBase
    {
        public ObservableCollection<TickerViewModel> Tickers { get; } = new ObservableCollection<TickerViewModel>();
    }
}