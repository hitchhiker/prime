using System;
using Prime.Core;
using Prime.Core.Exchange.Rates;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ExchangeRateResultViewModel : VmBase
    {
        private readonly ExchangeRateViewModel _model;
        private readonly ExchangeRateCollected _collected;

        public ExchangeRateResultViewModel(){}

        public ExchangeRateResultViewModel(ExchangeRateViewModel model, ExchangeRateCollected collected)
        {
            _model = model;
            _collected = collected;
            ConvertRight = (double)((decimal)collected.Price * (decimal)_model.ConvertLeft);
            FinalPrice = _model.ConvertLeft;
            AssetLeft = _model.AssetLeft;
            AssetRight = _model.AssetRight;
            UtcCreated = collected.UtcCreated;
            IsConverted = collected.IsConverted;


            if (IsConverted)
            {
                AssetConvert = collected.AssetConvert;
                InfoConvert = $"1 {AssetConvert.ShortCode} = {collected.Price.ToString()}";
                InfoLeft = $"1 {AssetLeft.ShortCode} = {collected.PriceConvert.ToString()}";
                InfoRight = $"1 {AssetRight.ShortCode} = {new Money(collected.Price, AssetLeft).ToString()}";
                InfoConvertFinal = $"1 {AssetLeft.ShortCode} = {new Money(collected.Price, AssetRight).ToString()}";
            }
            else
            {
                InfoLeft = $"1 {AssetLeft.ShortCode} = {collected.Price.ToString()}";
                InfoRight = $"1 {AssetRight.ShortCode} = {new Money(1 / collected.Price, AssetLeft).ToString()}";
            }

            IsVisible = true;
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        private double _finalPrice;
        public double FinalPrice
        {
            get => _finalPrice;
            set => Set(ref _finalPrice, value);
        }

        private double _convertRight;
        public double ConvertRight
        {
            get => _convertRight;
            set => Set(ref _convertRight, value);
        }

        private double _convertMiddle;
        public double ConvertMiddle
        {
            get => _convertMiddle;
            set => Set(ref _convertMiddle, value);
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

        private Asset _assetConvert;
        public Asset AssetConvert
        {
            get => _assetConvert;
            set => Set(ref _assetConvert, value);
        }

        private string _infoLeft;
        public string InfoLeft
        {
            get => _infoLeft;
            set => Set(ref _infoLeft, value);
        }

        private string _infoConvert;
        public string InfoConvert
        {
            get => _infoConvert;
            set => Set(ref _infoConvert, value);
        }
        private string _infoConvertFinal;
        public string InfoConvertFinal
        {
            get => _infoConvertFinal;
            set => Set(ref _infoConvertFinal, value);
        }

        private string _infoRight;
        public string InfoRight
        {
            get => _infoRight;
            set => Set(ref _infoRight, value);
        }

        private bool _isConverted;
        public bool IsConverted
        {
            get => _isConverted;
            set => Set(ref _isConverted, value);
        }

        private DateTime _utcCreated;
        public DateTime UtcCreated
        {
            get => _utcCreated;
            set => Set(ref _utcCreated, value);
        }
    }
}