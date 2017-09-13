namespace Prime.Ui.Wpf.ViewModel
{
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