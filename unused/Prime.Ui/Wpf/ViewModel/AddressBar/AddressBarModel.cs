using GalaSoft.MvvmLight.Messaging;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AddressBarModel : VmBase
    {
        public AddressBarModel() { }

        public AddressBarModel(ScreenViewModel screenViewModel) : base()
        {
            _screenViewModel = screenViewModel;
            AllAssetsViewModel = new AllAssetsViewModel() {SetAsDefault = true};
            AssetSelectorViewModel = new AssetSelectorViewModel();
            AddressBoxModel = new AddressBoxModel(screenViewModel, this);
        }

        private readonly ScreenViewModel _screenViewModel;

        public AddressBoxModel AddressBoxModel { get; }

        public AllAssetsViewModel AllAssetsViewModel { get; }
        public AssetSelectorViewModel AssetSelectorViewModel { get; }
    }
}