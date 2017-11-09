using System.Collections.ObjectModel;

namespace Prime.Ui.Wpf.ViewModel
{
    public class TickerViewModel : VmBase
    {
        public ObservableCollection<TickerItemBaseViewModel> Documents { get; } = new ObservableCollection<TickerItemBaseViewModel>();
    }
}