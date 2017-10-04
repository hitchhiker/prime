using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Prime.Core;
using Prime.Core.Exchange.Rates;
using Prime.Utility;
using Prime.Core.Exchange.Model;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ExchangeRateViewModel : DocumentPaneViewModel
    {
        public ExchangeRateViewModel()
        {
        }

        public ExchangeRateViewModel(ScreenViewModel model)
        {
            _assetLeft = Assets.I.GetRaw("BTC");
            _assetRight = UserContext.Current.BaseAsset;

            _dispatcher = Dispatcher.CurrentDispatcher;
            _debounceDispatcher = new DebounceDispatcher();

            ScreenViewModel = model;
            AllAssetsViewModel = new AllAssetsViewModel(model);

            foreach (var i in UserContext.Current.UserSettings.FavouritePairs)
                _requests.Add(_coord.AddRequest(i));

            foreach (var i in UserContext.Current.UserSettings.HistoricExchangeRates)
                _requests.Add(_coord.AddRequest(i));

            _coord.Messenger.Register<ExchangeRateCollected>(this, NewRate);

            GoCommand = new RelayCommand(Go);
        }

        private readonly Dispatcher _dispatcher;
        private readonly List<ExchangeRateRequest> _requests = new List<ExchangeRateRequest>();
        private readonly ExchangeRatesCoordinator _coord = ExchangeRatesCoordinator.I;
        private readonly DebounceDispatcher _debounceDispatcher;

        public ScreenViewModel ScreenViewModel;
        public AllAssetsViewModel AllAssetsViewModel { get; }

        public RelayCommand GoCommand { get; }

        private void NewRate(ExchangeRateCollected obj)
        {
            _dispatcher.Invoke(() =>
            {
                var results = _coord.Results();
                ExchangeRates.Clear();
                foreach (var er in results)
                    ExchangeRates.Add(er);

                if (AssetLeft.IsNone() || AssetRight.IsNone() || ConvertLeft == 0)
                    return;

                var ap = new AssetPair(AssetLeft, AssetRight);
                var exr = results.FirstOrDefault(x => x.Pair.Equals(ap));
                if (exr == null)
                    return;
                
                 ResultViewModel = new ExchangeRateResultViewModel(this, exr);
            });
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("exchange rates");
        }

        private DateTime _conversionDate = DateTime.Now;
        public DateTime ConversionDate
        {
            get => _conversionDate;
            set => Set(ref _conversionDate, value);
        }

        private double _convertLeft;
        public double ConvertLeft
        {
            get => _convertLeft;
            set => Set(ref _convertLeft, value);
        }

        private double _convertRight;
        public double ConvertRight
        {
            get => _convertRight;
            set => Set(ref _convertRight, value);
        }

        private Asset _assetLeft;
        public Asset AssetLeft
        {
            get => _assetLeft;
            set => Set(ref _assetLeft, value);
        }

        private Asset _assetRight;
        public Asset AssetRight
        {
            get => _assetRight;
            set => Set(ref _assetRight, value);
        }


        private ExchangeRateResultViewModel _resultViewModel = new ExchangeRateResultViewModel();
        public ExchangeRateResultViewModel ResultViewModel
        {
            get => _resultViewModel;
            set => Set(ref _resultViewModel, value);
        }

        private void Go()
        {
            _debounceDispatcher.Debounce(600, o => Convert());
        }

        private void Convert()
        {
            if (AssetRight.IsNone() || AssetLeft.IsNone())
                return;

            ConversionDate = DateTime.Now;
            ResultViewModel = new ExchangeRateResultViewModel();
            _requests.Add(_coord.AddRequest(new AssetPair(AssetLeft, AssetRight)));
        }

        public BindingList<ExchangeRateCollected> ExchangeRates { get; } = new BindingList<ExchangeRateCollected>();

        public override void Dispose()
        {
            foreach (var r in _requests)
                _coord.RemoveRequest(r);

            _coord.Messenger.Unregister<ExchangeRateCollected>(this, NewRate);

            base.Dispose();
        }
    }
}
