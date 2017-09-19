using GalaSoft.MvvmLight.Messaging;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AddressBarModel : VmBase
    {
        public AddressBarModel() { }

        public AddressBarModel(IMessenger messenger, ScreenViewModel screenViewModel) : base(messenger)
        {
            _screenViewModel = screenViewModel;
            AllAssetsViewModel = new AllAssetsViewModel(screenViewModel) {SetAsDefault = true};
            AddressBoxModel = new AddressBoxModel(messenger, screenViewModel, this);
        }

        private readonly ScreenViewModel _screenViewModel;

        public AddressBoxModel AddressBoxModel { get; }

        public AllAssetsViewModel AllAssetsViewModel { get; }
    }
}