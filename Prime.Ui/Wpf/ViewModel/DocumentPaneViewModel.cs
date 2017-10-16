using GalaSoft.MvvmLight;
using Prime.Core;

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

        public override void Dispose() { }
    }
}