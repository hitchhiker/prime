using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Ui.Wpf.View.Ticker;

namespace Prime.Ui.Wpf.ViewModel.Ticker
{
    public class TickerViewModel : VmBase
    {
        public ObservableCollection<TickerItemBaseViewModel> ListTickerItems { get; } = new ObservableCollection<TickerItemBaseViewModel>();
    }
}