using GalaSoft.MvvmLight;
using Prime.Common;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    public abstract class DocumentPaneViewModel : PaneViewModel, ICanGenerateCommand
    {
        private bool _canClose = true;
        public bool CanClose
        {
            get => _canClose;
            set => Set(ref _canClose, value);
        }

        private string _key;

        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }

        public abstract CommandContent GetPageCommand();

        private StarViewModel _starViewModel;
        public StarViewModel StarViewModel => _starViewModel ?? (_starViewModel = new StarViewModel(this));

        public override void Dispose()
        {
            _starViewModel?.Dispose();
        }
    }
}