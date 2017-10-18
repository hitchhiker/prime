﻿using System;
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
            AllAssetsViewModel = new AllAssetsViewModel();

            if (IsInDesignMode)
                return;

            _assetLeft = Assets.I.GetRaw("BTC");
            _assetRight = UserContext.Current.QuoteAsset;

            _dispatcher = PrimeWpf.I.UiDispatcher;
            _debouncer = new DebouncerThread(_dispatcher);
            

            foreach (var i in UserContext.Current.UserSettings.FavouritePairs)
                _requests.Add(_coord.AddRequest(this, i));

            foreach (var i in UserContext.Current.UserSettings.HistoricExchangeRates)
                _requests.Add(_coord.AddRequest(this, i));

            _coord.Messenger.Register<LatestPriceResult>(this, NewRate);

            GoCommand = new RelayCommand(AddRequestDebounced);
        }

        private readonly Dispatcher _dispatcher;
        private readonly List<LatestPriceRequest> _requests = new List<LatestPriceRequest>();
        private readonly LatestPriceCoordinator _coord = LatestPriceCoordinator.I;
        private readonly DebouncerThread _debouncer;
        public AllAssetsViewModel AllAssetsViewModel { get; }

        public RelayCommand GoCommand { get; }

        private void NewRate(LatestPriceResult result)
        {
            _dispatcher.Invoke(() =>
            {
                var e = ExchangeRates.FirstOrDefault(x => x.IsMatch(result));
                if (e != null)
                    ExchangeRates.Remove(e);

                ExchangeRates.Add(result);

                if (AssetLeft.IsNone() || AssetRight.IsNone() || ConvertLeft == 0)
                    return;

                var ap = new AssetPair(AssetLeft, AssetRight);
                if (!result.Pair.Equals(ap))
                    return;

                _isConverted = result.IsConverted;
                ResultViewModel = new ExchangeRateResultViewModel(this, result);
                LoadingInfo = "";
            });
        }

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("exchange rates");
        }

        private DateTime _conversionDate = DateTime.Now;
        public DateTime ConversionDate
        {
            get => _conversionDate;
            set => Set(ref _conversionDate, value);
        }

        private string _loadingInfo;
        public string LoadingInfo
        {
            get => _loadingInfo;
            set => Set(ref _loadingInfo, value);
        }

        private bool _isConverted;
        public bool IsConverted
        {
            get => _isConverted;
            set => Set(ref _isConverted, value);
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

        private void AddRequestDebounced()
        {
            _debouncer.Debounce(500, o => AddRequest());
        }

        private void AddRequest()
        {
            if (AssetRight.IsNone() || AssetLeft.IsNone())
                return;

            if (ConvertLeft == 0)
            {
                LoadingInfo = "";
            }
            else
            {
                LoadingInfo = "Converting " + new Money((decimal)ConvertLeft, AssetLeft) + " to " + AssetRight.ShortCode + "...";
            }

            ConversionDate = DateTime.Now;
            ResultViewModel = new ExchangeRateResultViewModel();
            _requests.Add(_coord.AddRequest(this, new AssetPair(AssetLeft, AssetRight)));
        }

        public BindingList<LatestPriceResult> ExchangeRates { get; } = new BindingList<LatestPriceResult>();

        public override void Dispose()
        {
            foreach (var r in _requests)
                _coord.RemoveRequest(this, r);

            _coord.Messenger.Unregister<LatestPriceResult>(this, NewRate);

            base.Dispose();
        }
    }
}
