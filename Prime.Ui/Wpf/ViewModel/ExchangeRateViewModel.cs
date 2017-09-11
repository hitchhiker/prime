using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ExchangeRateViewModel : DocumentPaneViewModel
    {
        public ExchangeRateViewModel() { }

        public ExchangeRateViewModel(ScreenViewModel model)
        {
            ExchangeConvertViewModel = new ExchangeConvertViewModel(model);
        }

        public ExchangeConvertViewModel ExchangeConvertViewModel { get; private set; }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("exchange rates");
        }
    }

    public class ExchangeConvertViewModel : VmBase
    {
        private string _convertLeft;
        private string _convertRight;

        public ExchangeConvertViewModel(ScreenViewModel model)
        {
            AllAssetsViewModel = new AllAssetsViewModel(model);
        }

        public AllAssetsViewModel AllAssetsViewModel { get; }

        public string ConvertLeft
        {
            get => _convertLeft;
            set => Set(ref _convertLeft, value);
        }

        public string ConvertRight
        {
            get => _convertRight;
            set => Set(ref _convertRight, value);
        }
    }
}
